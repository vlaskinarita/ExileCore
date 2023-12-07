namespace ExileCore.PoEMemory.MemoryObjects;

public class BestiaryCapturableMonster : RemoteMemoryObject
{
	private BestiaryCapturableMonster bestiaryCapturableMonsterKey;

	private BestiaryGenus bestiaryGenus;

	private BestiaryGroup bestiaryGroup;

	private string monsterName;

	private MonsterVariety monsterVariety;

	public int Id { get; set; }

	public string MonsterName
	{
		get
		{
			if (monsterName == null)
			{
				return monsterName = base.M.ReadStringU(base.M.Read<long>(base.Address + 32));
			}
			return monsterName;
		}
	}

	public MonsterVariety MonsterVariety
	{
		get
		{
			if (monsterVariety == null)
			{
				return monsterVariety = base.TheGame.Files.MonsterVarieties.GetByAddress(base.M.Read<long>(base.Address + 8));
			}
			return monsterVariety;
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

	public long BestiaryEncountersPtr => base.M.Read<long>(base.Address + 48);

	public BestiaryCapturableMonster BestiaryCapturableMonsterKey
	{
		get
		{
			if (bestiaryCapturableMonsterKey == null)
			{
				return bestiaryCapturableMonsterKey = base.TheGame.Files.BestiaryCapturableMonsters.GetByAddress(base.M.Read<long>(base.Address + 106));
			}
			return bestiaryCapturableMonsterKey;
		}
	}

	public BestiaryGenus BestiaryGenus
	{
		get
		{
			if (bestiaryGenus == null)
			{
				return bestiaryGenus = base.TheGame.Files.BestiaryGenuses.GetByAddress(base.M.Read<long>(base.Address + 97));
			}
			return bestiaryGenus;
		}
	}

	public int AmountCaptured => base.TheGame.IngameState.ServerData.GetBeastCapturedAmount(this);

	public override string ToString()
	{
		return $"Nane: {MonsterName}, Group: {BestiaryGroup.Name}, Family: {BestiaryGroup.Family.Name}, Captured: {AmountCaptured}";
	}
}
