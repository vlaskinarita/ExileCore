using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class Stats : Component
{
	private readonly CachedValue<StatsComponentOffsets> _statsValue;

	private readonly CachedValue<SubStatsComponentOffsets> _substructStatsValue;

	private readonly CachedValue<Dictionary<GameStat, int>> _statDictionary;

	private readonly Dictionary<string, int> testHumanDictionary = new Dictionary<string, int>();

	private readonly Dictionary<GameStat, int> statDictionary = new Dictionary<GameStat, int>();

	public new long OwnerAddress => _statsValue.Value.Owner;

	public SubStatsComponentOffsets StatsComponent => _substructStatsValue.Value;

	public Dictionary<GameStat, int> StatDictionary => _statDictionary.Value;

	public long StatsCount => StatsComponent.Stats.TotalElements(8);

	public Stats()
	{
		_statsValue = new FrameCache<StatsComponentOffsets>(() => base.M.Read<StatsComponentOffsets>(base.Address));
		_substructStatsValue = new FrameCache<SubStatsComponentOffsets>(() => base.M.Read<SubStatsComponentOffsets>(_statsValue.Value.SubStatsPtr));
		_statDictionary = new FrameCache<Dictionary<GameStat, int>>(ParseStats);
	}

	public Dictionary<GameStat, int> ParseStats()
	{
		if (base.Address == 0L)
		{
			return statDictionary;
		}
		long statsCount = StatsCount;
		if (statsCount <= 0 || StatsComponent.Stats.Last > StatsComponent.Stats.End || StatsComponent.Stats.First == 0L)
		{
			return statDictionary;
		}
		if (statsCount > 9000)
		{
			Core.Logger?.Error($"Stats over capped: {StatsComponent.Stats} Total Stats: {statsCount}");
			return statDictionary;
		}
		(GameStat, int)[] array = base.M.ReadStdVector<(GameStat, int)>(StatsComponent.Stats);
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

	public Dictionary<string, int> HumanStats()
	{
		Dictionary<GameStat, int> dictionary = StatDictionary;
		testHumanDictionary.Clear();
		StatsDat stats = base.TheGame.Files.Stats;
		if (stats == null)
		{
			return null;
		}
		foreach (KeyValuePair<GameStat, int> item in dictionary)
		{
			if (stats.recordsById.TryGetValue((int)item.Key, out var value))
			{
				testHumanDictionary[value.Key] = item.Value;
			}
		}
		return testHumanDictionary;
	}
}
