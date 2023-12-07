using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.FilesInMemory;

public class PassiveSkill : RemoteMemoryObject
{
	private string id;

	private string name;

	private int passiveId = -1;

	private List<(StatsDat.StatRecord, int)> stats;

	public int PassiveId
	{
		get
		{
			if (passiveId == -1)
			{
				return passiveId = base.M.Read<int>(base.Address + 48);
			}
			return passiveId;
		}
	}

	public string Id => id ?? (id = base.M.ReadStringU(base.M.Read<long>(base.Address), 255));

	public string Name => name ?? (name = base.M.ReadStringU(base.M.Read<long>(base.Address + 52), 255));

	public string Icon => base.M.ReadStringU(base.M.Read<long>(base.Address + 8), 255);

	public IEnumerable<(StatsDat.StatRecord, int)> Stats
	{
		get
		{
			if (stats == null)
			{
				stats = new List<(StatsDat.StatRecord, int)>();
				int size = base.M.Read<int>(base.Address + 16);
				(long, long)[] source = base.M.ReadMem<(long, long)>(base.M.Read<long>(base.Address + 24), size);
				stats = source.Select(((long, long) x, int i) => (base.TheGame.Files.Stats.GetStatByAddress(x.Item1), ReadStatValue(i))).ToList();
			}
			return stats;
		}
	}

	private int ReadStatValue(int index)
	{
		return base.M.Read<int>(base.Address + 32 + index * 4);
	}

	public override string ToString()
	{
		return $"{Name}, Id: {Id}, PassiveId: {PassiveId}";
	}
}
