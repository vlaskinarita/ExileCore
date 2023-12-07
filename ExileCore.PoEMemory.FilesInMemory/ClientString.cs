namespace ExileCore.PoEMemory.FilesInMemory;

public class ClientString : RemoteMemoryObject
{
	private string _id;

	private string _description;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string Text => _description ?? (_description = base.M.ReadStringU(base.M.Read<long>(base.Address + 8)));

	public override string ToString()
	{
		return $"{Id} {Text} ({base.Address:X})";
	}
}
