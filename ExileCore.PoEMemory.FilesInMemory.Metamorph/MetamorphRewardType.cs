namespace ExileCore.PoEMemory.FilesInMemory.Metamorph;

public class MetamorphRewardType : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address), 255);

	public string Art => base.M.ReadStringU(base.M.Read<long>(base.Address + 8), 255);

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 16), 255);

	public override string ToString()
	{
		return Id + ": " + Name;
	}
}
