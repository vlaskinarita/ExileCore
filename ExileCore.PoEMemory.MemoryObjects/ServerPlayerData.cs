using ExileCore.Shared.Enums;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ServerPlayerData : StructuredRemoteMemoryObject<ServerPlayerDataOffsets>
{
	public CharacterClass Class => (CharacterClass)(base.Structure.PlayerClass & 0xF);

	public int Level => base.Structure.CharacterLevel;

	public int PassiveRefundPointsLeft => base.Structure.PassiveRefundPointsLeft;

	public int QuestPassiveSkillPoints => base.Structure.QuestPassiveSkillPoints;

	public int FreePassiveSkillPointsLeft => base.Structure.FreePassiveSkillPointsLeft;

	public int TotalAscendencyPoints => base.Structure.TotalAscendencyPoints;

	public int SpentAscendencyPoints => base.Structure.SpentAscendencyPoints;

	public NativePtrArray AllocatedPassivesIds => base.Structure.PassiveSkillIds;
}
