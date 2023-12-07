namespace ExileCore.PoEMemory.FilesInMemory.Atlas;

public class AtlasRegion : RemoteMemoryObject
{
	public int Index { get; internal set; }

	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public int Unknown => base.M.Read<int>(base.Address + 16);

	public override string ToString()
	{
		return Name + " (" + Id + ")";
	}
}
