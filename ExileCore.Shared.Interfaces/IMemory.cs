using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ExileCore.PoEMemory;
using ExileCore.Shared.Enums;
using GameOffsets.Native;

namespace ExileCore.Shared.Interfaces;

public interface IMemory : IDisposable
{
	IntPtr MainWindowHandle { get; }

	IntPtr OpenProcessHandle { get; }

	long AddressOfProcess { get; }

	Dictionary<OffsetsName, long> BaseOffsets { get; }

	Process Process { get; }

	MemoryBackendMode BackendMode { get; set; }

	string ReadString(long addr, int length = 256, bool replaceNull = true);

	string ReadNativeString(long addr);

	string ReadStringU(long addr, int lengthBytes = 256, bool replaceNull = true);

	byte[] ReadMem(long addr, int size);

	byte[] ReadMem(IntPtr addr, int size);

	T[] ReadMem<T>(long addr, int size) where T : unmanaged;

	T[] ReadMem<T>(IntPtr address, int size) where T : unmanaged;

	byte[] ReadBytes(long addr, int size);

	byte[] ReadBytes(long addr, long size);

	List<T> ReadStructsArray<T>(long startAddress, long endAddress, int structSize, RemoteMemoryObject game) where T : RemoteMemoryObject, new();

	List<T> ReadStructsArray<T>(long startAddress, long endAddress, int structSize) where T : unmanaged;

	IList<T> ReadDoublePtrVectorClasses<T>(long address, RemoteMemoryObject game, bool noNullPointers = false) where T : RemoteMemoryObject, new();

	IList<long> ReadPointersArray(long startAddress, long endAddress, int offset = 8);

	IList<long> ReadSecondPointerArray_Count(long startAddress, int count);

	T Read<T>(Pointer addr, params int[] offsets) where T : unmanaged;

	T Read<T>(IntPtr addr, params int[] offsets) where T : unmanaged;

	T Read<T>(long addr, params int[] offsets) where T : unmanaged;

	T Read<T>(Pointer addr) where T : unmanaged;

	T Read<T>(IntPtr addr) where T : unmanaged;

	T Read<T>(long addr) where T : unmanaged;

	IList<T> ReadNativeArray<T>(INativePtrArray ptrArray, int offset = 8) where T : unmanaged;

	IList<T> ReadNativeArray<T>(long first, long last, int offset = 8) where T : unmanaged;

	IList<Tuple<long, int>> ReadDoublePointerIntList(long address);

	IList<T> ReadList<T>(IntPtr head) where T : unmanaged;

	IList<T> ReadStdList<T>(IntPtr head) where T : unmanaged;

	T[] ReadRMOStdVector<T>(StdVector nativeContainer, int objectSize) where T : RemoteMemoryObject, new();

	T[] ReadStdVector<T>(StdVector nativeContainer) where T : unmanaged;

	T[] ReadStdVectorStride<T>(StdVector nativeContainer, int stride) where T : unmanaged;

	T ReadStdVectorElement<T>(StdVector nativeContainer, int index) where T : unmanaged;

	IList<long> ReadListPointer(IntPtr head);

	long[] FindPatterns(params IPattern[] patterns);

	bool IsInvalid();

	void NotifyFrame();

	T[] ReadStdVector<T>(NativePtrArray nativeContainer) where T : unmanaged;

	T[] ReadStdVectorStride<T>(NativePtrArray nativeContainer, int stride) where T : unmanaged;

	T ReadStdVectorElement<T>(NativePtrArray nativeContainer, int index) where T : unmanaged;
}
