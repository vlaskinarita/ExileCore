using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects.Heist;

public class HeistJobRecord : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public string RequiredSkillIcon => base.M.ReadStringU(base.M.Read<long>(base.Address + 16));

	public string SkillIcon => base.M.ReadStringU(base.M.Read<long>(base.Address + 24));

	public string MapIcon => base.M.ReadStringU(base.M.Read<long>(base.Address + 40));

	public StatsDat.StatRecord LevelStat => base.TheGame.Files.Stats.GetStatByAddress(base.M.Read<long>(base.Address + 48));

	public StatsDat.StatRecord AlertStat => base.TheGame.Files.Stats.GetStatByAddress(base.M.Read<long>(base.Address + 64));

	public StatsDat.StatRecord AlarmStat => base.TheGame.Files.Stats.GetStatByAddress(base.M.Read<long>(base.Address + 80));

	public StatsDat.StatRecord CostStat => base.TheGame.Files.Stats.GetStatByAddress(base.M.Read<long>(base.Address + 96));

	public StatsDat.StatRecord ExperienceGainStat => base.TheGame.Files.Stats.GetStatByAddress(base.M.Read<long>(base.Address + 112));

	public override string ToString()
	{
		return Name;
	}
}
