namespace ExileCore.PoEMemory.FilesInMemory;

public class BlightTowerDat : RemoteMemoryObject
{
	private string _id;

	private string _name;

	private string _description;

	private string _icon;

	private int? _radius;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string Name => _name ?? (_name = base.M.ReadStringU(base.M.Read<long>(base.Address + 8)));

	public string Description => _description ?? (_description = base.M.ReadStringU(base.M.Read<long>(base.Address + 16)));

	public string Icon => _icon ?? (_icon = base.M.ReadStringU(base.M.Read<long>(base.Address + 24)));

	public int Radius
	{
		get
		{
			int valueOrDefault = _radius.GetValueOrDefault();
			if (!_radius.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 60);
				_radius = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public override string ToString()
	{
		return $"{Id} {Name} ({base.Address:X})";
	}
}
