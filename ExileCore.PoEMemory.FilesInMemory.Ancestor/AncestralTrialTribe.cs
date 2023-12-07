namespace ExileCore.PoEMemory.FilesInMemory.Ancestor;

public class AncestralTrialTribe : RemoteMemoryObject
{
	private string _id;

	private string _name;

	private string _nameTribe;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string NameTribe => _nameTribe ?? (_nameTribe = base.M.ReadStringU(base.M.Read<long>(base.Address + 40)));

	public string Name => _name ?? (_name = base.M.ReadStringU(base.M.Read<long>(base.Address + 72)));

	public override string ToString()
	{
		return $"{Id} {Name} {base.Address:X}";
	}
}
