using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class Mods : Component
{
	private readonly CachedValue<ModsComponentOffsets> _cachedValue;

	private readonly CachedValue<ModsComponentStatsOffsets> _cachedStatsStruct;

	public ModsComponentOffsets ModsStruct => _cachedValue.Value;

	public string UniqueName => GetUniqueName(ModsStruct.UniqueName);

	public bool Identified
	{
		get
		{
			if (base.Address != 0L)
			{
				return ModsStruct.Identified;
			}
			return false;
		}
	}

	public ItemRarity ItemRarity
	{
		get
		{
			if (base.Address == 0L)
			{
				return ItemRarity.Normal;
			}
			return (ItemRarity)ModsStruct.ItemRarity;
		}
	}

	public long Hash => ModsStruct.implicitMods.GetHashCode() ^ ModsStruct.explicitMods.GetHashCode() ^ ModsStruct.GetHashCode();

	public List<ItemMod> ItemMods => ImplicitMods.Concat(ExplicitMods).Concat(EnchantedMods).Concat(ScourgeMods)
		.Concat(CrucibleMods)
		.ToList();

	public List<ItemMod> ImplicitMods => GetMods(ModsStruct.implicitMods.First, ModsStruct.implicitMods.Last);

	public List<ItemMod> ExplicitMods => GetMods(ModsStruct.explicitMods.First, ModsStruct.explicitMods.Last);

	public List<ItemMod> EnchantedMods => GetMods(ModsStruct.enchantMods.First, ModsStruct.enchantMods.Last);

	public List<ItemMod> ScourgeMods => GetMods(ModsStruct.ScourgeMods.First, ModsStruct.ScourgeMods.Last);

	public List<ItemMod> CrucibleMods => GetMods(ModsStruct.crucibleMods.First, ModsStruct.crucibleMods.Last);

	public List<ItemMod> FracturedMods
	{
		get
		{
			if (CountFractured <= 0)
			{
				return new List<ItemMod>();
			}
			return ExplicitMods.Take(CountFractured).ToList();
		}
	}

	public List<ItemMod> SynthesisMods => ImplicitMods.Where((ItemMod mod) => mod.ModRecord.Key.StartsWith("SynthesisImplicit")).ToList();

	public int ItemLevel
	{
		get
		{
			if (base.Address == 0L)
			{
				return 1;
			}
			return ModsStruct.ItemLevel;
		}
	}

	public int RequiredLevel
	{
		get
		{
			if (base.Address == 0L)
			{
				return 1;
			}
			return ModsStruct.RequiredLevel;
		}
	}

	public bool IsUsable
	{
		get
		{
			if (base.Address != 0L)
			{
				return ModsStruct.IsUsable == 1;
			}
			return false;
		}
	}

	public bool IsMirrored
	{
		get
		{
			if (base.Address != 0L)
			{
				return ModsStruct.IsMirrored == 1;
			}
			return false;
		}
	}

	public int CountFractured => ModsStruct.FracturedModsCount;

	public bool Synthesised => CountSynthesised > 0;

	public int CountSynthesised => SynthesisMods.Count;

	public bool HaveFractured => CountFractured > 0;

	public ItemStats ItemStats => new ItemStats(base.Owner);

	public List<string> HumanStats => GetStats(_cachedStatsStruct.Value.ExplicitStatsArray);

	public List<string> HumanCraftedStats => GetStats(_cachedStatsStruct.Value.CraftedStatsArray);

	public List<string> HumanImpStats => GetStats(_cachedStatsStruct.Value.ImplicitStatsArray);

	public List<string> FracturedStats => GetStats(_cachedStatsStruct.Value.FracturedStatsArray);

	public List<string> EnchantedStats => GetStats(_cachedStatsStruct.Value.EnchantedStatsArray);

	public List<string> CrucibleStats => GetStats(_cachedStatsStruct.Value.CrucibleStatsArray);

	public ushort IncubatorKills => ModsStruct.IncubatorKills;

	public string IncubatorName
	{
		get
		{
			if (base.Address == 0L || ModsStruct.IncubatorPtr == 0L)
			{
				return null;
			}
			return base.M.ReadStringU(base.M.Read<long>(ModsStruct.IncubatorPtr, new int[1] { 32 }));
		}
	}

	public Mods()
	{
		_cachedValue = new FrameCache<ModsComponentOffsets>(() => base.M.Read<ModsComponentOffsets>(base.Address));
		_cachedStatsStruct = new FrameCache<ModsComponentStatsOffsets>(() => base.M.Read<ModsComponentStatsOffsets>(_cachedValue.Value.ModsComponentStatsPtr));
	}

	private List<string> GetStats(NativePtrArray array)
	{
		IList<long> list = base.M.ReadPointersArray(array.First, array.Last, ModsComponentOffsets.HumanStats);
		List<string> list2 = new List<string>();
		foreach (long pointer in list)
		{
			list2.Add(RemoteMemoryObject.Cache.StringCache.Read($"{"Mods"}{pointer}", () => base.M.ReadStringU(pointer, 512)));
		}
		return list2;
	}

	private List<ItemMod> GetMods(long startOffset, long endOffset)
	{
		List<ItemMod> list = new List<ItemMod>();
		if (base.Address == 0L)
		{
			return list;
		}
		if ((endOffset - startOffset) / ItemMod.STRUCT_SIZE > 12)
		{
			return list;
		}
		for (long num = startOffset; num < endOffset; num += ItemMod.STRUCT_SIZE)
		{
			list.Add(GetObject<ItemMod>(num));
		}
		return list;
	}

	private string GetUniqueName(NativePtrArray source)
	{
		List<string> words = new List<string>();
		if (base.Address == 0L || source.Size / 8 > 1000)
		{
			return string.Empty;
		}
		for (long num = source.First; num < source.Last; num += ModsComponentOffsets.NameRecordSize)
		{
			words.Add(base.M.ReadStringU(base.M.Read<long>(num, new int[1] { ModsComponentOffsets.NameOffset })).Trim());
		}
		return RemoteMemoryObject.Cache.StringCache.Read($"{"Mods"}{source.First}", () => string.Join(" ", words.ToArray()));
	}
}
