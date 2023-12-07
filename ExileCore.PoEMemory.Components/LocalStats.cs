using System.Collections.Generic;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class LocalStats : Component
{
	private readonly CachedValue<LocalStatsComponentOffsets> _localStatsValue;

	private readonly CachedValue<Dictionary<GameStat, int>> _statDictionary;

	private readonly Dictionary<GameStat, int> statDictionary = new Dictionary<GameStat, int>();

	public Dictionary<GameStat, int> StatDictionary => _statDictionary.Value;

	public LocalStats()
	{
		_localStatsValue = new FrameCache<LocalStatsComponentOffsets>(() => base.M.Read<LocalStatsComponentOffsets>(base.Address));
		_statDictionary = new FrameCache<Dictionary<GameStat, int>>(ParseStats);
	}

	public Dictionary<GameStat, int> ParseStats()
	{
		if (base.Address == 0L)
		{
			return statDictionary;
		}
		(GameStat, int)[] array = base.M.ReadStdVector<(GameStat, int)>(_localStatsValue.Value.StatsPtr);
		statDictionary.Clear();
		statDictionary.EnsureCapacity(array.Length);
		(GameStat, int)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			var (key, value) = array2[i];
			statDictionary[key] = value;
		}
		return statDictionary;
	}

	public int GetStatValue(GameStat stat)
	{
		if (StatDictionary.TryGetValue(stat, out var value))
		{
			return value;
		}
		return 0;
	}
}
