namespace ExileCore.PoEMemory.MemoryObjects.Heist;

public class HeistChestRecord : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address, new int[1]));

	public override string ToString()
	{
		return $"{Id} ({base.Address:X})";
	}
}
