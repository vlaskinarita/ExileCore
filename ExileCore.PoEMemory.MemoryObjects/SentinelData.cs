namespace ExileCore.PoEMemory.MemoryObjects;

public class SentinelData : RemoteMemoryObject
{
	public SentinelState State => base.M.Read<SentinelState>(base.Address + 24);
}
