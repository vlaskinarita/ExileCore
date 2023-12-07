namespace ExileCore.PoEMemory.MemoryObjects;

public class Quest : RemoteMemoryObject
{
	private string _id;

	private string _name;

	private string _icon;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address), 255));

	public int Act => base.M.Read<int>(base.Address + 8);

	public string Name => _name ?? (_name = base.M.ReadStringU(base.M.Read<long>(base.Address + 12)));

	public string Icon => _icon ?? (_icon = base.M.ReadStringU(base.M.Read<long>(base.Address + 20)));

	public override string ToString()
	{
		return "Id: " + Id + ", Name: " + Name;
	}
}
