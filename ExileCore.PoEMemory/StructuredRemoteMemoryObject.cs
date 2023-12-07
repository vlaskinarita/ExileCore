using ExileCore.Shared.Cache;

namespace ExileCore.PoEMemory;

public abstract class StructuredRemoteMemoryObject<T> : RemoteMemoryObject where T : unmanaged
{
	private readonly CachedValue<T> _cachedStructValue;

	public T Structure => _cachedStructValue.Value;

	public StructuredRemoteMemoryObject()
	{
		_cachedStructValue = CreateStructFrameCache<T>();
	}
}
