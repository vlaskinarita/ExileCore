namespace ExileCore.PoEMemory.FilesInMemory;

public class Character : RemoteMemoryObject
{
	private string _id;

	private string _name;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string Name => _name ?? (_name = base.M.ReadStringU(base.M.Read<long>(base.Address + 8)));

	public override string ToString()
	{
		return $"{Name} {Id} ({base.Address:X})";
	}
}
