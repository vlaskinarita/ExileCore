namespace ExileCore.PoEMemory.FilesInMemory;

public class ItemVisualIdentity : RemoteMemoryObject
{
	private string _id;

	private string _artPath;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string ArtPath => _artPath ?? (_artPath = base.M.ReadStringU(base.M.Read<long>(base.Address + 8)));

	public override string ToString()
	{
		return Id + " (" + ArtPath + ")";
	}
}
