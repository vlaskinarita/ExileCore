using System;
using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects;

public class GrantedEffectsPerLevel : RemoteMemoryObject
{
	private GrantedEffectPerLevel Effect => base.TheGame.Files.GrantedEffectsPerLevel.GetByAddress(base.Address);

	public SkillGemWrapper SkillGemWrapper => ReadObject<SkillGemWrapper>(base.Address);

	public int Level => Effect.Level;

	public int RequiredLevel => Effect.RequiredLevel;

	public int ManaMultiplier => Effect.CostMultiplier;

	public int Cooldown => Effect.Cooldown;

	public int ManaCost => base.M.Read<int>(base.Address + 168);

	public int EffectivenessOfAddedDamage => base.M.Read<int>(base.Address + 172);

	public IEnumerable<Tuple<StatsDat.StatRecord, int>> Stats
	{
		get
		{
			List<Tuple<StatsDat.StatRecord, int>> list = new List<Tuple<StatsDat.StatRecord, int>>();
			int num = base.M.Read<int>(base.Address + 20);
			long num2 = base.M.Read<long>(base.Address + 28);
			num2 += 8;
			for (int i = 0; i < num; i++)
			{
				long address = base.M.Read<long>(num2);
				StatsDat.StatRecord statByAddress = base.TheGame.Files.Stats.GetStatByAddress(address);
				list.Add(new Tuple<StatsDat.StatRecord, int>(statByAddress, ReadStatValue(i)));
				num2 += 16;
			}
			return list;
		}
	}

	public IEnumerable<string> Tags
	{
		get
		{
			List<string> list = new List<string>();
			int num = base.M.Read<int>(base.Address + 68);
			long num2 = base.M.Read<long>(base.Address + 76);
			num2 += 8;
			for (int i = 0; i < num; i++)
			{
				long addr = base.M.Read<long>(num2);
				addr = base.M.Read<long>(addr);
				list.Add(base.M.ReadStringU(addr));
				num2 += 16;
			}
			return list;
		}
	}

	public IEnumerable<Tuple<StatsDat.StatRecord, int>> QualityStats
	{
		get
		{
			List<Tuple<StatsDat.StatRecord, int>> list = new List<Tuple<StatsDat.StatRecord, int>>();
			int num = base.M.Read<int>(base.Address + 132);
			long num2 = base.M.Read<long>(base.Address + 140);
			num2 += 8;
			for (int i = 0; i < num; i++)
			{
				long address = base.M.Read<long>(num2);
				StatsDat.StatRecord statByAddress = base.TheGame.Files.Stats.GetStatByAddress(address);
				list.Add(new Tuple<StatsDat.StatRecord, int>(statByAddress, ReadQualityStatValue(i)));
				num2 += 16;
			}
			return list;
		}
	}

	public IEnumerable<StatsDat.StatRecord> TypeStats
	{
		get
		{
			List<StatsDat.StatRecord> list = new List<StatsDat.StatRecord>();
			int num = base.M.Read<int>(base.Address + 188);
			long num2 = base.M.Read<long>(base.Address + 196);
			num2 += 8;
			for (int i = 0; i < num; i++)
			{
				long address = base.M.Read<long>(num2);
				StatsDat.StatRecord statByAddress = base.TheGame.Files.Stats.GetStatByAddress(address);
				list.Add(statByAddress);
				num2 += 16;
			}
			return list;
		}
	}

	internal int ReadStatValue(int index)
	{
		return base.M.Read<int>(base.Address + 84 + index * 4);
	}

	internal int ReadQualityStatValue(int index)
	{
		return base.M.Read<int>(base.Address + 156 + index * 4);
	}
}
