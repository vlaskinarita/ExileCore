namespace ExileCore.PoEMemory.MemoryObjects;

public class SkillGemWrapper : RemoteMemoryObject
{
	private const int ActiveSkillOffset = 99;

	private const int ActiveSkillSubIdOffset1 = 107;

	private const int ActiveSkillSubIdOffset2 = 48;

	private const int ActiveSkillSubIdRecordLength = 233;

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public ActiveSkillWrapper ActiveSkill => ReadObject<ActiveSkillWrapper>(base.Address + 99);

	public long ActiveSkillSubId => (ActiveSkill.Address - base.M.Read<long>(base.Address + 107, new int[2] { 48, 0 })) / 233;
}
