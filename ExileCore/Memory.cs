using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ExileCore.PoEMemory;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using GameOffsets.Native;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;

namespace ExileCore;

public class Memory : IMemory, IDisposable
{
	private bool closed;

	private readonly Stopwatch sw = Stopwatch.StartNew();

	private IMemoryBackend _backend;

	public MemoryBackendMode BackendMode
	{
		get
		{
			IMemoryBackend backend = _backend;
			if (!(backend is DefaultMemoryBackend))
			{
				if (backend is PagedMemoryBackend)
				{
					return MemoryBackendMode.CacheAndPreload;
				}
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(backend);
				MemoryBackendMode result = default(MemoryBackendMode);
				return result;
			}
			return MemoryBackendMode.AlwaysRead;
		}
		set
		{
			if (BackendMode != value)
			{
				IMemoryBackend memoryBackend = default(IMemoryBackend);
				switch (value)
				{
				case MemoryBackendMode.CacheAndPreload:
					memoryBackend = new PagedMemoryBackend(new DefaultMemoryBackend(OpenProcessHandle));
					break;
				case MemoryBackendMode.AlwaysRead:
					memoryBackend = new DefaultMemoryBackend(OpenProcessHandle);
					break;
				default:
					global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(value);
					break;
				}
				IMemoryBackend backend = memoryBackend;
				IMemoryBackend backend2 = _backend;
				_backend = backend;
				backend2?.Dispose();
			}
		}
	}

	public IntPtr MainWindowHandle { get; }

	public IntPtr OpenProcessHandle { get; }

	public long AddressOfProcess { get; }

	public Dictionary<OffsetsName, long> BaseOffsets { get; }

	public Process Process { get; }

	public Memory((Process, Offsets) tuple)
	{
		Process = tuple.Item1;
		AddressOfProcess = Process.MainModule.BaseAddress.ToInt64();
		MainWindowHandle = Process.MainWindowHandle;
		OpenProcessHandle = NativeWrapper.OpenProcess(ProcessAccessFlags.Read, Process.Id);
		_backend = new DefaultMemoryBackend(OpenProcessHandle);
		BaseOffsets = tuple.Item2.DoPatternScans(this);
	}

	public void NotifyFrame()
	{
		_backend.NotifyFrame();
	}

	public string ReadString(long addr, int length = 256, bool replaceNull = true)
	{
		if (addr <= 65536 && addr >= -1)
		{
			return string.Empty;
		}
		if (length <= 0 || length > 1000000)
		{
			return "TextTooLong";
		}
		Span<byte> span = ((length >= 1024) ? ((Span<byte>)new byte[length]) : stackalloc byte[length]);
		Span<byte> span2 = span;
		if (!ReadMem(new IntPtr(addr), span2))
		{
			return string.Empty;
		}
		string @string = Encoding.UTF8.GetString(span2);
		if (!replaceNull)
		{
			return @string;
		}
		return RTrimNull(@string);
	}

	public string ReadNativeString(long address)
	{
		uint num = Read<uint>(address + 16);
		if (num == 0)
		{
			return string.Empty;
		}
		if (8 <= num)
		{
			long addr = Read<long>(address);
			return ReadStringU(addr);
		}
		return ReadStringU(address);
	}

	public string ReadStringU(long addr, int lengthBytes = 256, bool replaceNull = true)
	{
		if (lengthBytes > Limits.UnicodeStringLength || lengthBytes < 0)
		{
			return string.Empty;
		}
		if (addr == 0L)
		{
			return string.Empty;
		}
		Span<byte> span = ((lengthBytes >= 1024) ? ((Span<byte>)new byte[lengthBytes]) : stackalloc byte[lengthBytes]);
		Span<byte> span2 = span;
		if (!ReadMem(new IntPtr(addr), span2))
		{
			return string.Empty;
		}
		if (span2[0] == 0 && span2[1] == 0)
		{
			return string.Empty;
		}
		string @string = Encoding.Unicode.GetString(span2);
		if (!replaceNull)
		{
			return @string;
		}
		return RTrimNull(@string);
	}

	public byte[] ReadMem(long addr, int size)
	{
		return ReadMem(new IntPtr(addr), size);
	}

	public byte[] ReadMem(IntPtr address, int size)
	{
		return ReadMem<byte>(address, size);
	}

	public T[] ReadMem<T>(long addr, int size) where T : unmanaged
	{
		return ReadMem<T>(new IntPtr(addr), size);
	}

	public T[] ReadMem<T>(IntPtr address, int size) where T : unmanaged
	{
		try
		{
			if (size <= 0 || address.ToInt64() <= 0 || size >= int.MaxValue / Unsafe.SizeOf<T>())
			{
				return Array.Empty<T>();
			}
			T[] array = new T[size];
			_backend.TryReadMemory(address, MemoryMarshal.Cast<T, byte>(array));
			return array;
		}
		catch (Exception value)
		{
			DebugWindow.LogError($"Readmem-> A: {address} Size: {size}. {value}");
			throw;
		}
	}

	public bool ReadMem<T>(IntPtr address, Span<T> target) where T : unmanaged
	{
		try
		{
			if (target.Length <= 0 || address.ToInt64() <= 0 || target.Length >= int.MaxValue / Unsafe.SizeOf<T>())
			{
				return false;
			}
			return _backend.TryReadMemory(address, MemoryMarshal.Cast<T, byte>(target));
		}
		catch (Exception value)
		{
			DebugWindow.LogError($"Readmem-> A: {address} Size: {target.Length}. {value}");
			throw;
		}
	}

	public byte[] ReadBytes(long addr, int size)
	{
		return ReadMem(addr, size);
	}

	public byte[] ReadBytes(long addr, long size)
	{
		return ReadMem(addr, (int)size);
	}

	public List<T> ReadStructsArray<T>(long startAddress, long endAddress, int structSize, RemoteMemoryObject game) where T : RemoteMemoryObject, new()
	{
		List<T> list = new List<T>();
		long num = (endAddress - startAddress) / structSize;
		if (num < 0 || num > Limits.ReadStructsArrayCount)
		{
			DebugWindow.LogError($"Maybe overflow memory in {"ReadStructsArray"} for reading structures of type: {typeof(T).Name}, start is {startAddress}, end is {endAddress}, size is {structSize}", 3f);
			return list;
		}
		for (long num2 = startAddress; num2 < endAddress; num2 += structSize)
		{
			list.Add(RemoteMemoryObject.GetObjectStatic<T>(num2));
		}
		return list;
	}

	public List<T> ReadStructsArray<T>(long startAddress, long endAddress, int structSize) where T : unmanaged
	{
		List<T> list = new List<T>();
		int num = 0;
		long num2 = (endAddress - startAddress) / structSize;
		if (num2 < 0 || num2 > Limits.ReadStructsArrayCount)
		{
			DebugWindow.LogError("Maybe overflow memory in ReadStructsArray for reading structures of type: " + typeof(T).Name, 3f);
			return list;
		}
		for (long num3 = startAddress; num3 < endAddress; num3 += structSize)
		{
			list.Add(Read<T>(num3));
			num++;
			if (num > Limits.ReadStructsArrayCount)
			{
				DebugWindow.LogError("Maybe overflow memory in ReadStructsArray for reading structures of type: " + typeof(T).Name, 3f);
				return list;
			}
		}
		return list;
	}

	public IList<T> ReadDoublePtrVectorClasses<T>(long address, RemoteMemoryObject game, bool noNullPointers = false) where T : RemoteMemoryObject, new()
	{
		long num = Read<long>(address);
		int num2 = (int)(Read<long>(address + 16) - num);
		byte[] value = ReadMem(new IntPtr(num), num2);
		List<T> list = new List<T>();
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < num2; i += 16)
		{
			if (stopwatch.ElapsedMilliseconds > Limits.ReadMemoryTimeLimit)
			{
				DebugWindow.LogError($"ReadDoublePtrVectorClasses error result count: {list.Count}");
				return new List<T>();
			}
			long num3 = BitConverter.ToInt64(value, i);
			if (!(num3 == 0 && noNullPointers))
			{
				list.Add(game.GetObject<T>(num3));
			}
		}
		return list;
	}

	public IList<long> ReadPointersArray(long startAddress, long endAddress, int offset = 8)
	{
		List<long> result = new List<long>();
		long num = endAddress - startAddress;
		if (endAddress <= 0 || startAddress <= 0 || num <= 0 || num > 160000 || num % 8 != 0L)
		{
			return result;
		}
		sw.Restart();
		result = new List<long>((int)(num / offset) + 1);
		byte[] value = ReadMem(startAddress, (int)num);
		for (int i = 0; i < num; i += offset)
		{
			if (sw.ElapsedMilliseconds > Limits.ReadMemoryTimeLimit)
			{
				DebugWindow.LogError($"ReadPointersArray error result count: {result.Count}");
				return new List<long>();
			}
			result.Add(BitConverter.ToInt64(value, i));
		}
		return result;
	}

	public IList<long> ReadSecondPointerArray_Count(long startAddress, int count)
	{
		throw new NotImplementedException();
	}

	public T Read<T>(Pointer addr, params int[] offsets) where T : unmanaged
	{
		throw new NotImplementedException();
	}

	public T Read<T>(IntPtr addr, params int[] offsets) where T : unmanaged
	{
		if (addr == IntPtr.Zero)
		{
			return default(T);
		}
		long num = Read<long>(addr);
		for (int i = 0; i < offsets.Length - 1; i++)
		{
			if (num == 0L)
			{
				return default(T);
			}
			int num2 = offsets[i];
			num = Read<long>(num + num2);
		}
		if (num == 0L)
		{
			return default(T);
		}
		return Read<T>(num + offsets[^1]);
	}

	public T Read<T>(long addr, params int[] offsets) where T : unmanaged
	{
		return Read<T>(new IntPtr(addr), offsets);
	}

	public T Read<T>(Pointer addr) where T : unmanaged
	{
		throw new NotImplementedException();
	}

	public T Read<T>(IntPtr addr) where T : unmanaged
	{
		if (addr == IntPtr.Zero)
		{
			return default(T);
		}
		Span<T> span = stackalloc T[1];
		_backend.TryReadMemory(addr, MemoryMarshal.Cast<T, byte>(span));
		return span[0];
	}

	public T Read<T>(long addr) where T : unmanaged
	{
		IntPtr addr2 = new IntPtr(addr);
		return Read<T>(addr2);
	}

	public IList<Tuple<long, int>> ReadDoublePointerIntList(long address)
	{
		List<Tuple<long, int>> list = new List<Tuple<long, int>>();
		long num = Read<long>(address);
		NativeListNode nativeListNode = Read<NativeListNode>(num);
		list.Add(new Tuple<long, int>(nativeListNode.Ptr2_Key, nativeListNode.Value));
		Stopwatch stopwatch = Stopwatch.StartNew();
		while (num != nativeListNode.Next)
		{
			if (stopwatch.ElapsedMilliseconds > Limits.ReadMemoryTimeLimit)
			{
				Core.Logger?.Error($"ReadDoublePointerIntList error result count: {list.Count}");
				return new List<Tuple<long, int>>();
			}
			nativeListNode = Read<NativeListNode>(nativeListNode.Next);
			list.Add(new Tuple<long, int>(nativeListNode.Ptr2_Key, nativeListNode.Value));
		}
		if (list.Count > 0)
		{
			list.RemoveAt(list.Count - 1);
		}
		return list;
	}

	public IList<T> ReadList<T>(IntPtr head) where T : unmanaged
	{
		List<T> list = new List<T>();
		NativeListNode nativeListNode = Read<NativeListNode>(head);
		Stopwatch stopwatch = Stopwatch.StartNew();
		long num = head.ToInt64();
		while (num != nativeListNode.Next)
		{
			if (stopwatch.ElapsedMilliseconds > Limits.ReadMemoryTimeLimit)
			{
				Core.Logger?.Error($"Readlist error result count: {list.Count}");
				return new List<T>();
			}
			list.Add(Read<T>(nativeListNode.Next));
			nativeListNode = Read<NativeListNode>(nativeListNode.Next);
		}
		return list;
	}

	public IList<T> ReadStdList<T>(IntPtr head) where T : unmanaged
	{
		List<T> list = new List<T>();
		IntPtr next = Read<StdListNode>(head).Next;
		while (next != head)
		{
			StdListNode<T> stdListNode = Read<StdListNode<T>>(next);
			if (next == IntPtr.Zero)
			{
				Core.Logger?.Error("Terminating reading of list next nodes because of unexpected 0x00 found. This is normal if it happens after closing the game, otherwise report it.");
				break;
			}
			list.Add(stdListNode.Data);
			next = stdListNode.Next;
		}
		return list;
	}

	public T[] ReadRMOStdVector<T>(StdVector nativeContainer, int objectSize) where T : RemoteMemoryObject, new()
	{
		long num = nativeContainer.Last - nativeContainer.First;
		long num2 = num / objectSize;
		if (num <= 0 || num % objectSize != 0L || num2 > Array.MaxLength)
		{
			return Array.Empty<T>();
		}
		return (from x in Enumerable.Range(0, (int)num2)
			select RemoteMemoryObject.GetObjectStatic<T>(nativeContainer.First + x * objectSize)).ToArray();
	}

	public T[] ReadStdVector<T>(StdVector nativeContainer) where T : unmanaged
	{
		int num = Unsafe.SizeOf<T>();
		long num2 = nativeContainer.Last - nativeContainer.First;
		long num3 = num2 / num;
		if (num2 <= 0 || num2 % num != 0L || num3 > Array.MaxLength)
		{
			return Array.Empty<T>();
		}
		return ReadMem<T>(nativeContainer.First, (int)num3);
	}

	public T[] ReadStdVectorStride<T>(StdVector nativeContainer, int stride) where T : unmanaged
	{
		int num = Unsafe.SizeOf<T>();
		if (stride < num)
		{
			return Array.Empty<T>();
		}
		long num2 = nativeContainer.Last - nativeContainer.First;
		long num3 = nativeContainer.TotalElements(stride);
		if (num2 <= 0 || num2 % stride != 0L || num3 > Array.MaxLength)
		{
			return Array.Empty<T>();
		}
		byte[] array = ReadBytes(nativeContainer.First, num3 * stride);
		T[] array2 = new T[num3];
		for (int i = 0; i < num3; i++)
		{
			array2[i] = Unsafe.As<byte, T>(ref array[stride * i]);
		}
		return array2;
	}

	public T[] ReadStdVector<T>(NativePtrArray nativeContainer) where T : unmanaged
	{
		int num = Unsafe.SizeOf<T>();
		long num2 = nativeContainer.Last - nativeContainer.First;
		long num3 = nativeContainer.ElementCount(num);
		if (num2 <= 0 || num2 % num != 0L || num3 > Array.MaxLength)
		{
			return Array.Empty<T>();
		}
		return ReadMem<T>(nativeContainer.First, (int)num3);
	}

	public T[] ReadStdVectorStride<T>(NativePtrArray nativeContainer, int stride) where T : unmanaged
	{
		return ReadStdVectorStride<T>(new StdVector
		{
			First = nativeContainer.First,
			End = nativeContainer.End,
			Last = nativeContainer.Last
		}, stride);
	}

	public T ReadStdVectorElement<T>(StdVector nativeContainer, int index) where T : unmanaged
	{
		int num = Unsafe.SizeOf<T>();
		int num2 = (int)(nativeContainer.Last - nativeContainer.First) / num;
		if (index < 0 && index >= num2)
		{
			return default(T);
		}
		return Read<T>(nativeContainer.First + num * index);
	}

	public T ReadStdVectorElement<T>(NativePtrArray nativeContainer, int index) where T : unmanaged
	{
		int num = Unsafe.SizeOf<T>();
		long num2 = nativeContainer.ElementCount(num);
		if (index < 0 && index >= num2)
		{
			return default(T);
		}
		return Read<T>(nativeContainer.First + num * index);
	}

	public IList<long> ReadListPointer(IntPtr head)
	{
		List<long> list = new List<long>();
		NativeListNode nativeListNode = Read<NativeListNode>(head);
		Stopwatch stopwatch = Stopwatch.StartNew();
		long num = head.ToInt64();
		while (num != nativeListNode.Next)
		{
			if (stopwatch.ElapsedMilliseconds > Limits.ReadMemoryTimeLimit)
			{
				Core.Logger?.Error($"ReadListPointer error result count: {list.Count}");
				return new List<long>();
			}
			list.Add(nativeListNode.Next);
			nativeListNode = Read<NativeListNode>(nativeListNode.Next);
		}
		return list;
	}

	public long[] FindPatterns(params IPattern[] patterns)
	{
		byte[] exeImage = ReadMem(new IntPtr(AddressOfProcess), Process.MainModule.ModuleMemorySize);
		long[] address = new long[patterns.Length];
		Parallel.For(0L, patterns.Length, FindPattern);
		exeImage = null;
		return address;
		static bool CompareData(IPattern pattern, Span<byte> data)
		{
			byte[] bytes = pattern.Bytes;
			string mask = pattern.Mask;
			if (bytes[0] != data[0] || bytes[^1] != data[bytes.Length - 1])
			{
				return false;
			}
			for (int i = 0; i < bytes.Length; i++)
			{
				if (mask[i] == 'x' && bytes[i] != data[i])
				{
					return false;
				}
			}
			return true;
		}
		void FindPattern(long iPattern)
		{
			IPattern pattern2 = patterns[iPattern];
			int num = pattern2.Bytes.Length;
			bool flag = false;
			using (new PerformanceTimer("Pattern: " + pattern2.Name + " -> ", 0, delegate(string s, TimeSpan span)
			{
				DebugWindow.LogMsg($"{s}: Time: {span.TotalMilliseconds} ms. Offset:[{address[iPattern]}] Started searching offset with:{pattern2.StartOffset}");
			}, log: false))
			{
				for (int j = pattern2.StartOffset; j < exeImage.Length - num; j++)
				{
					if (CompareData(pattern2, exeImage.AsSpan(j)))
					{
						flag = true;
						address[iPattern] = j;
						break;
					}
				}
				if (!flag)
				{
					for (int k = 0; k < pattern2.StartOffset; k++)
					{
						if (CompareData(pattern2, exeImage.AsSpan(k)))
						{
							address[iPattern] = k;
							break;
						}
					}
				}
			}
		}
	}

	public bool IsInvalid()
	{
		if (!Process.HasExited)
		{
			return closed;
		}
		return true;
	}

	public IList<T> ReadNativeArray<T>(INativePtrArray ptrArray, int offset = 8) where T : unmanaged
	{
		return ReadNativeArray<T>(ptrArray.First.ToInt64(), ptrArray.Last.ToInt64(), offset);
	}

	public IList<T> ReadNativeArray<T>(long first, long last, int offset = 8) where T : unmanaged
	{
		if (first == 0L)
		{
			return new List<T>();
		}
		List<T> list = new List<T>((int)(last - first) / offset);
		foreach (long item in ReadPointersArray(first, last, offset))
		{
			list.Add(Read<T>(item));
		}
		return list;
	}

	public void Dispose()
	{
		if (!closed)
		{
			closed = true;
			try
			{
				NativeWrapper.CloseHandle(OpenProcessHandle);
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Error when dispose memory: " + ex.Message);
			}
		}
	}

	private static string RTrimNull(string text)
	{
		int num = text.IndexOf('\0');
		if (num <= 0)
		{
			return text;
		}
		return text.Substring(0, num);
	}
}
