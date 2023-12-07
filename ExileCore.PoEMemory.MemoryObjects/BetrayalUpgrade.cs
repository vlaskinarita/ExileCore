namespace ExileCore.PoEMemory.MemoryObjects;

public class BetrayalUpgrade : RemoteMemoryObject
{
	public string UpgradeName => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public string UpgradeStat => base.M.ReadStringU(base.M.Read<long>(base.Address + 16));

	public string Art => base.M.ReadStringU(base.M.Read<long>(base.Address + 40));

	public override string ToString()
	{
		return UpgradeName + " (" + UpgradeStat + ")";
	}
}
