using System;

namespace ExileCore;

public interface IMemoryBackend : IDisposable
{
	bool TryReadMemory(IntPtr address, Span<byte> target);

	void NotifyFrame();
}
