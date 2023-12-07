namespace ExileCore.PoEMemory.MemoryObjects;

public class BestiaryGenus : RemoteMemoryObject
{
	private BestiaryGroup bestiaryGroup;

	private string genusId;

	private string icon;

	private string name;

	private string name2;

	public int Id { get; internal set; }

	public string GenusId
	{
		get
		{
			if (genusId == null)
			{
				return genusId = base.M.ReadStringU(base.M.Read<long>(base.Address));
			}
			return genusId;
		}
	}

	public string Name
	{
		get
		{
			if (name == null)
			{
				return name = base.M.ReadStringU(base.M.Read<long>(base.Address + 8));
			}
			return name;
		}
	}

	public BestiaryGroup BestiaryGroup
	{
		get
		{
			if (bestiaryGroup == null)
			{
				return bestiaryGroup = base.TheGame.Files.BestiaryGroups.GetByAddress(base.M.Read<long>(base.Address + 24));
			}
			return bestiaryGroup;
		}
	}

	public string Name2
	{
		get
		{
			if (name2 == null)
			{
				return name2 = base.M.ReadStringU(base.M.Read<long>(base.Address + 32));
			}
			return name2;
		}
	}

	public string Icon
	{
		get
		{
			if (icon == null)
			{
				return icon = base.M.ReadStringU(base.M.Read<long>(base.Address + 40));
			}
			return icon;
		}
	}

	public int MaxInStorage => base.M.Read<int>(base.Address + 48);

	public override string ToString()
	{
		return $"{Name}, MaxInStorage: {MaxInStorage}";
	}
}
