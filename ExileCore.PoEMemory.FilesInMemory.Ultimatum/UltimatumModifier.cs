namespace ExileCore.PoEMemory.FilesInMemory.Ultimatum;

public class UltimatumModifier : RemoteMemoryObject
{
	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public override string ToString()
	{
		return Name;
	}
}
