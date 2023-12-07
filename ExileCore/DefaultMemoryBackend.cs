using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using ExileCore.Shared;
using ProcessMemoryUtilities.Managed;

namespace ExileCore;

public class DefaultMemoryBackend : IMemoryBackend, IDisposable
{
	private static readonly DebugInformation PerFrameStats = new DebugInformation("Memory reading");

	private long _currentFrameUsedTime;

	private readonly IntPtr _openProcessHandle;

	public DefaultMemoryBackend(IntPtr processHandle)
	{
		_openProcessHandle = processHandle;
	}

	public bool TryReadMemory(IntPtr address, Span<byte> target)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		byte[] array = ArrayPool<byte>.Shared.Rent(target.Length);
		try
		{
			bool result = NativeWrapper.ReadProcessMemoryArray(_openProcessHandle, address, array, 0, target.Length);
			array.AsSpan().Slice(0, target.Length).CopyTo(target);
			return result;
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(array);
			Interlocked.Add(ref _currentFrameUsedTime, stopwatch.ElapsedTicks);
		}
	}

	public void NotifyFrame()
	{
		long ticks = Interlocked.Exchange(ref _currentFrameUsedTime, 0L);
		PerFrameStats.Tick = new TimeSpan(ticks).TotalMilliseconds;
	}

	public void Dispose()
	{
	}
}
