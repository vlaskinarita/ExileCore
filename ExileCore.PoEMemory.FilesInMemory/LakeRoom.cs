namespace ExileCore.PoEMemory.FilesInMemory;

public class LakeRoom : RemoteMemoryObject
{
	private string _id;

	private string _description;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string Description => _description ?? (_description = base.M.ReadStringU(base.M.Read<long>(base.Address + 80)));

	public override string ToString()
	{
		return $"{Id} {Description} ({base.Address:X})";
	}
}
