using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.FilesInMemory;

public class ModsDat : FileInMemory
{
	public class ModRecord
	{
		private class LevelComparer : IComparer<ModRecord>
		{
			public int Compare(ModRecord x, ModRecord y)
			{
				return -x.MinLevel + y.MinLevel;
			}
		}

		public const int NumberOfStats = 6;

		public static IComparer<ModRecord> ByLevelComparer = new LevelComparer();

		public long Address { get; }

		public string Key { get; }

		public ModType AffixType { get; }

		public ModDomain Domain { get; }

		public string Group { get; }

		public List<string> Groups { get; }

		public int MinLevel { get; }

		public StatsDat.StatRecord[] StatNames { get; }

		public IntRange[] StatRange { get; }

		public IDictionary<string, int> TagChances { get; }

		public TagsDat.TagRecord[] Tags { get; }

		public int Hash { get; }

		public string UserFriendlyName { get; }

		public bool IsEssence { get; }

		public string Tier { get; }

		public string TypeName { get; }

		public ModRecord(IMemory m, StatsDat sDat, TagsDat tagsDat, long addr)
		{
			Address = addr;
			ModsRecordOffsets modsRecord = m.Read<ModsRecordOffsets>(addr);
			Key = RemoteMemoryObject.Cache.StringCache.Read($"{"ModsDat"}{modsRecord.Key.buf}", () => modsRecord.Key.ToString(m));
			Hash = modsRecord.Hash;
			MinLevel = modsRecord.MinLevel;
			long read = m.Read<long>(modsRecord.TypePtr);
			TypeName = RemoteMemoryObject.Cache.StringCache.Read($"{"ModsDat"}{read}", () => m.ReadStringU(read, 255));
			List<(StatsDat.StatRecord, IntRange)> source = new(StatsDat.StatRecord, IntRange)[6]
			{
				GetStat(modsRecord.Stat1Ptr, modsRecord.Stat1Range),
				GetStat(modsRecord.Stat2Ptr, modsRecord.Stat2Range),
				GetStat(modsRecord.Stat3Ptr, modsRecord.Stat3Range),
				GetStat(modsRecord.Stat4Ptr, modsRecord.Stat4Range),
				GetStat(modsRecord.Stat5Ptr, modsRecord.Stat5Range),
				GetStat(modsRecord.Stat6Ptr, modsRecord.Stat6Range)
			}.Where(((StatsDat.StatRecord Stat, IntRange Range) x) => x.Stat != null).ToList();
			StatNames = source.Select<(StatsDat.StatRecord, IntRange), StatsDat.StatRecord>(((StatsDat.StatRecord Stat, IntRange Range) x) => x.Stat).ToArray();
			Domain = (ModDomain)modsRecord.Domain;
			UserFriendlyName = RemoteMemoryObject.Cache.StringCache.Read($"{"ModsDat"}{modsRecord.UserFriendlyName}", () => m.ReadStringU(modsRecord.UserFriendlyName));
			AffixType = (ModType)modsRecord.AffixType;
			Group = RemoteMemoryObject.Cache.StringCache.Read($"{"ModsDat"}{modsRecord.Group}", () => string.Join(",", from x in m.ReadMem<(long, long)>(modsRecord.Group.Ptr, (int)modsRecord.Group.Count)
				select m.ReadStringU(m.Read<long>(x.Ptr)) into x
				orderby x
				select x));
			Groups = Group.Split(",").ToList();
			StatRange = source.Select<(StatsDat.StatRecord, IntRange), IntRange>(((StatsDat.StatRecord Stat, IntRange Range) x) => x.Range).ToArray();
			Tags = new TagsDat.TagRecord[modsRecord.Tags];
			long ta = modsRecord.ta;
			for (int j = 0; j < Tags.Length; j++)
			{
				long addr2 = ta + 16 * j;
				long i = m.Read<long>(addr2, new int[1]);
				string key = RemoteMemoryObject.Cache.StringCache.Read($"{"ModsDat"}{i}", () => m.ReadStringU(i, 255));
				Tags[j] = tagsDat.Records[key];
			}
			TagChances = new Dictionary<string, int>();
			long tc = modsRecord.tc;
			for (int k = 0; k < Tags.Length; k++)
			{
				TagChances[Tags[k].Key] = m.Read<int>(tc + 4 * k);
			}
			IsEssence = modsRecord.IsEssence == 1;
			Tier = RemoteMemoryObject.Cache.StringCache.Read($"{"ModsDat"}{modsRecord.Tier}", () => m.ReadStringU(modsRecord.Tier));
			(StatsDat.StatRecord Stat, IntRange Range) GetStat(long statPtr, Vector2i range)
			{
				long s = ((statPtr == 0L) ? 0 : m.Read<long>(statPtr));
				if (s == 0L)
				{
					return (null, new IntRange(0, 0));
				}
				string key2 = RemoteMemoryObject.Cache.StringCache.Read($"{"ModsDat"}{s}", () => m.ReadStringU(s));
				return (sDat.records[key2], new IntRange(range.X, range.Y));
			}
		}

		public override string ToString()
		{
			return $"Name: {UserFriendlyName}, Key: {Key}, MinLevel: {MinLevel}";
		}
	}

	public IDictionary<string, ModRecord> records { get; } = new Dictionary<string, ModRecord>(StringComparer.OrdinalIgnoreCase);


	public IDictionary<long, ModRecord> DictionaryRecords { get; } = new Dictionary<long, ModRecord>();


	public IDictionary<Tuple<string, ModType>, List<ModRecord>> recordsByTier { get; } = new Dictionary<Tuple<string, ModType>, List<ModRecord>>();


	public ModsDat(IMemory m, Func<long> address, StatsDat sDat, TagsDat tagsDat)
		: base(m, address)
	{
		loadItems(sDat, tagsDat);
	}

	public ModRecord GetModByAddress(long address)
	{
		DictionaryRecords.TryGetValue(address, out var value);
		return value;
	}

	private void loadItems(StatsDat sDat, TagsDat tagsDat)
	{
		foreach (long item in RecordAddresses())
		{
			ModRecord modRecord;
			try
			{
				modRecord = new ModRecord(base.M, sDat, tagsDat, item);
			}
			catch (Exception exception)
			{
				Logger.Log.Warning(exception, "Error load ModRecord");
				continue;
			}
			if (records.ContainsKey(modRecord.Key))
			{
				continue;
			}
			DictionaryRecords.Add(item, modRecord);
			records.Add(modRecord.Key, modRecord);
			if (modRecord.Domain != ModDomain.Monster)
			{
				Tuple<string, ModType> key = Tuple.Create(modRecord.Group, modRecord.AffixType);
				if (!recordsByTier.TryGetValue(key, out var value))
				{
					value = new List<ModRecord>();
					recordsByTier[key] = value;
				}
				value.Add(modRecord);
			}
		}
		foreach (List<ModRecord> value2 in recordsByTier.Values)
		{
			value2.Sort(ModRecord.ByLevelComparer);
		}
	}
}
