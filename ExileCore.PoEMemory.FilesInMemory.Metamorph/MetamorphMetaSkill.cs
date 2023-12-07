using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.FilesInMemory.Metamorph;

public class MetamorphMetaSkill : RemoteMemoryObject
{
	public MonsterVariety MonsterVarietyMetadata => base.TheGame.Files.MonsterVarieties.GetByAddress(base.M.Read<long>(base.Address + 8));

	public MetamorphMetaSkillType MetaSkill => base.TheGame.Files.MetamorphMetaSkillTypes.GetByAddress(base.M.Read<long>(base.Address + 24));

	public string SkillName => base.M.ReadStringU(base.M.Read<long>(base.Address + 232), 255);

	public string GrantedEffect1 => base.M.ReadStringU(base.M.Read<long>(base.Address + 40, new int[1]), 255);

	public string GrantedEffect2 => base.M.ReadStringU(base.M.Read<long>(base.Address + 88, new int[1]), 255);

	public override string ToString()
	{
		return $"{MetaSkill}, {MonsterVarietyMetadata?.VarietyId}";
	}
}
