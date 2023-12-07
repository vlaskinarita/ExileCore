namespace ExileCore.PoEMemory.FilesInMemory.Ancestor;

public class AncestralTrialItem : RemoteMemoryObject
{
	private string _id;

	private string _name;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string Name => _name ?? (_name = base.M.ReadStringU(base.M.Read<long>(base.Address + 12)));

	public override string ToString()
	{
		return $"{Id} {Name} {base.Address:X}";
	}
}
