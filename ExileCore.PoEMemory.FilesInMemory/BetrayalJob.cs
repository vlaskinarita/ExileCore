namespace ExileCore.PoEMemory.FilesInMemory;

public class BetrayalJob : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public string Art => base.M.ReadStringU(base.M.Read<long>(base.Address + 32));

	public override string ToString()
	{
		return Name;
	}
}
