namespace ExileCore.PoEMemory.FilesInMemory;

public class BetrayalChoice : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public override string ToString()
	{
		return Name;
	}
}
