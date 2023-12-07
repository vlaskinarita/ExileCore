namespace ExileCore.PoEMemory.MemoryObjects;

public class BestiaryGroup : RemoteMemoryObject
{
	private BestiaryFamily family;

	private string groupId;

	private string name;

	public int Id { get; internal set; }

	public string GroupId
	{
		get
		{
			if (groupId == null)
			{
				return groupId = base.M.ReadStringU(base.M.Read<long>(base.Address));
			}
			return groupId;
		}
	}

	public string Description => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public string Illustration => base.M.ReadStringU(base.M.Read<long>(base.Address + 16));

	public string Name
	{
		get
		{
			if (name == null)
			{
				return name = base.M.ReadStringU(base.M.Read<long>(base.Address + 24));
			}
			return name;
		}
	}

	public string SmallIcon => base.M.ReadStringU(base.M.Read<long>(base.Address + 32));

	public string ItemIcon => base.M.ReadStringU(base.M.Read<long>(base.Address + 40));

	public BestiaryFamily Family
	{
		get
		{
			if (family == null)
			{
				return family = base.TheGame.Files.BestiaryFamilies.GetByAddress(base.M.Read<long>(base.Address + 56));
			}
			return family;
		}
	}

	public override string ToString()
	{
		return Name;
	}
}
