namespace ExileCore.PoEMemory.FilesInMemory.Ancestor;

public class AncestralTrialUnit : RemoteMemoryObject
{
	private string _id;

	private string _name;

	private string _description;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string Name => _name ?? (_name = base.M.ReadStringU(base.M.Read<long>(base.Address + 8)));

	public string Description => _description ?? (_description = base.M.ReadStringU(base.M.Read<long>(base.Address + 56), 5120));

	public override string ToString()
	{
		return $"{Id} {Name} {base.Address:X}";
	}
}
