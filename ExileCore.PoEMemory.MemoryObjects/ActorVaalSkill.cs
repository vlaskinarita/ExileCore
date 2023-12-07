using System;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ActorVaalSkill : RemoteMemoryObject
{
	private const int NAMES_POINTER_OFFSET = 0;

	private const int INTERNAL_NAME_OFFSET = 0;

	private const int NAME_OFFSET = 8;

	private const int DESCRIPTION_OFFSET = 16;

	private const int SKILL_NAME_OFFSET = 24;

	private const int ICON_OFFSET = 32;

	private const int MAX_VAAL_SOULS_OFFSET = 16;

	private const int CURRENT_VAAL_SOULS_OFFSET = 20;

	private long NamesPointer => base.M.Read<long>(base.Address);

	public string VaalSkillInternalName => base.M.ReadStringU(base.M.Read<long>(NamesPointer));

	public string VaalSkillDisplayName => base.M.ReadStringU(base.M.Read<long>(NamesPointer + 8));

	public string VaalSkillDescription => base.M.ReadStringU(base.M.Read<long>(NamesPointer + 16));

	public string VaalSkillSkillName => base.M.ReadStringU(base.M.Read<long>(NamesPointer + 24));

	public string VaalSkillIcon => base.M.ReadStringU(base.M.Read<long>(NamesPointer + 32));

	public int VaalMaxSouls => base.M.Read<int>(base.Address + 16);

	[Obsolete("Use VaalMaxSouls instead")]
	public int VaalSoulsPerUse => VaalMaxSouls;

	public int CurrVaalSouls => base.M.Read<int>(base.Address + 20);
}
