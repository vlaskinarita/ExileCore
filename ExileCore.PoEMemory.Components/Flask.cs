using System.Collections.Generic;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class Flask : Component
{
	private readonly FrameCache<LocalStats> cacheLocalStatsComponent;

	private readonly FrameCache<Quality> cacheQualityComponent;

	private readonly CachedValue<Dictionary<GameStat, int>> _flaskStatDictionary;

	private readonly Dictionary<GameStat, int> flaskStatDictionary = new Dictionary<GameStat, int>();

	public LocalStats LocalStatsComponent => cacheLocalStatsComponent.Value;

	public Quality QualityComponent => cacheQualityComponent.Value;

	public Dictionary<GameStat, int> FlaskStatDictionary => _flaskStatDictionary.Value;

	public int LifeRecover
	{
		get
		{
			float num = GetStatValue(GameStat.LocalFlaskLifeToRecover);
			float num2 = LocalStatsComponent.GetStatValue(GameStat.LocalFlaskLifeToRecoverPct);
			return (int)(((double)(float)LocalStatsComponent.GetStatValue(GameStat.LocalFlaskAmountToRecoverPct) * 0.009999999 + 1.0) * (((double)QualityComponent.ItemQuality * 0.01 + 1.0) * (double)num * (double)(num2 * 0.01f + 1f)) + 0.5);
		}
	}

	public int ManaRecover
	{
		get
		{
			float num = GetStatValue(GameStat.LocalFlaskManaToRecover);
			float num2 = LocalStatsComponent.GetStatValue(GameStat.LocalFlaskManaToRecoverPct);
			return (int)(((double)(float)LocalStatsComponent.GetStatValue(GameStat.LocalFlaskAmountToRecoverPct) * 0.009999999 + 1.0) * (((double)QualityComponent.ItemQuality * 0.01 + 1.0) * (double)num * (double)(num2 * 0.01f + 1f)) + 0.5);
		}
	}

	public Flask()
	{
		cacheQualityComponent = new FrameCache<Quality>(() => ReadObjectAt<Quality>(48));
		cacheLocalStatsComponent = new FrameCache<LocalStats>(() => ReadObjectAt<LocalStats>(56));
		_flaskStatDictionary = new FrameCache<Dictionary<GameStat, int>>(ParseStats);
	}

	public Dictionary<GameStat, int> ParseStats()
	{
		if (base.Address == 0L)
		{
			return flaskStatDictionary;
		}
		(GameStat, int)[] array = base.M.ReadStdVector<(GameStat, int)>(base.M.Read<StdVector>(base.M.Read<long>(base.Address + 40) + 48));
		flaskStatDictionary.Clear();
		flaskStatDictionary.EnsureCapacity(array.Length);
		(GameStat, int)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			var (key, value) = array2[i];
			flaskStatDictionary[key] = value;
		}
		return flaskStatDictionary;
	}

	public int GetStatValue(GameStat stat)
	{
		if (FlaskStatDictionary.TryGetValue(stat, out var value))
		{
			return value;
		}
		return 0;
	}
}
