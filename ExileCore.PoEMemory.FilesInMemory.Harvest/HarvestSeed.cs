namespace ExileCore.PoEMemory.FilesInMemory.Harvest;

public class HarvestSeed : RemoteMemoryObject
{
	private string _id;

	private int? _tier;

	private int? _type;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public int Tier
	{
		get
		{
			int valueOrDefault = _tier.GetValueOrDefault();
			if (!_tier.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 24);
				_tier = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public int Type
	{
		get
		{
			int valueOrDefault = _type.GetValueOrDefault();
			if (!_type.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 84);
				_type = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public override string ToString()
	{
		return $"{Id} {Tier} {Type} {base.Address:X}";
	}
}
