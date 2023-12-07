using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class MonsterVariety : RemoteMemoryObject
{
	private string _varietyId;

	public int Id { get; internal set; }

	public string VarietyId => _varietyId ?? (_varietyId = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public long MonsterTypePtr => base.M.Read<long>(base.Address + 8);

	public int ObjectSize => base.M.Read<int>(base.Address + 28);

	public int MinimumAttackDistance => base.M.Read<int>(base.Address + 32);

	public int MaximumAttackDistance => base.M.Read<int>(base.Address + 36);

	public List<string> ACTFiles => (from x in base.M.Read<DatArrayStruct>(base.Address + 40).ReadDatPtr(base.M)
		select base.M.ReadStringU(x)).ToList();

	public List<string> AOFiles => (from x in base.M.Read<DatArrayStruct>(base.Address + 56).ReadDatPtr(base.M)
		select base.M.ReadStringU(x)).ToList();

	public string BaseMonsterTypeIndex => base.M.ReadStringU(base.M.Read<long>(base.Address + 72));

	public IEnumerable<ModsDat.ModRecord> Mods => (from x in base.M.Read<DatArrayStruct>(base.Address + 80).ReadDatPtr(base.M).Select(base.TheGame.Files.Mods.GetModByAddress)
		where x != null
		select x).ToList();

	public int ModelSizeMultiplier => base.M.Read<int>(base.Address + 116);

	public int ExperienceMultiplier => base.M.Read<int>(base.Address + 156);

	public int CriticalStrikeChance => base.M.Read<int>(base.Address + 188);

	public string AISFile => base.M.ReadStringU(base.M.Read<long>(base.Address + 212));

	public string MonsterName => base.M.ReadStringU(base.M.Read<long>(base.Address + 260));

	public int DamageMultiplier => base.M.Read<int>(base.Address + 268);

	public int LifeMultiplier => base.M.Read<int>(base.Address + 272);

	public int AttackSpeed => base.M.Read<int>(base.Address + 276);

	public override string ToString()
	{
		return $"Name: {MonsterName}, VarietyId: {VarietyId}, BaseMonsterTypeIndex: {BaseMonsterTypeIndex}";
	}
}
