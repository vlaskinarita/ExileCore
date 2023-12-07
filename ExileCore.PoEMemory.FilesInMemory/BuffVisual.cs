namespace ExileCore.PoEMemory.FilesInMemory;

public class BuffVisual : RemoteMemoryObject
{
	private string _id;

	private string _ddsFile;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string DdsFile => _ddsFile ?? (_ddsFile = base.M.ReadStringU(base.M.Read<long>(base.Address + 8)));

	public override string ToString()
	{
		return $"{Id} {DdsFile} ({base.Address:X})";
	}
}
