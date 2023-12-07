using System;
using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects.Heist;

public class HeistNpcRecord : RemoteMemoryObject
{
	private long _JobCount => Math.Max(0L, Math.Min(10L, base.M.Read<long>(base.Address + 32)));

	public List<HeistJobRecord> Jobs => GetJobs(base.M.Read<long>(base.Address + 40));

	public string PortraitFile => base.M.ReadStringU(base.M.Read<long>(base.Address + 48));

	private int _StatCount => Math.Clamp(base.M.Read<int>(base.Address + 56), 0, 32);

	public List<StatsDat.StatRecord> Stats => GetStats(base.M.Read<long>(base.Address + 64), _StatCount);

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 108));

	private List<StatsDat.StatRecord> GetStats(long start, int count)
	{
		List<StatsDat.StatRecord> list = new List<StatsDat.StatRecord>();
		int num = 0;
		while (num < count)
		{
			list.Add(base.TheGame.Files.Stats.GetStatByAddress(base.M.Read<long>(start, new int[1])));
			num++;
			start += 16;
		}
		return list;
	}

	private List<HeistJobRecord> GetJobs(long source)
	{
		List<HeistJobRecord> list = new List<HeistJobRecord>();
		if ((source += 8) == 0L)
		{
			return list;
		}
		int num = 0;
		while (num < _JobCount)
		{
			list.Add(base.TheGame.Files.HeistJobs.GetByAddress(base.M.Read<long>(source)));
			num++;
			source += 16;
		}
		return list;
	}

	public override string ToString()
	{
		return Name;
	}
}
