namespace ExileCore.PoEMemory.MemoryObjects;

public class BestiaryFamily : RemoteMemoryObject
{
	private string familyId;

	private string name;

	public int Id { get; internal set; }

	public string FamilyId
	{
		get
		{
			if (familyId == null)
			{
				return familyId = base.M.ReadStringU(base.M.Read<long>(base.Address));
			}
			return familyId;
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

	public string Icon => base.M.ReadStringU(base.M.Read<long>(base.Address + 16));

	public string SmallIcon => base.M.ReadStringU(base.M.Read<long>(base.Address + 24));

	public string Illustration => base.M.ReadStringU(base.M.Read<long>(base.Address + 32));

	public string PageArt => base.M.ReadStringU(base.M.Read<long>(base.Address + 40));

	public string Description => base.M.ReadStringU(base.M.Read<long>(base.Address + 48));

	public override string ToString()
	{
		return Name;
	}
}
