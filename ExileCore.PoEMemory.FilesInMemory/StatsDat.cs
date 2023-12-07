using System;
using System.Collections.Generic;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class StatsDat : FileInMemory
{
	public class StatRecord
	{
		public GameStat MatchingStat { get; }

		public string Key { get; }

		public long Address { get; }

		public StatType Type { get; }

		public bool Flag0 { get; }

		public bool IsLocal { get; }

		public bool IsWeaponLocal { get; }

		public bool Flag3 { get; }

		public string UserFriendlyName { get; }

		public int ID { get; }

		public StatRecord(IMemory m, long addr, int iCounter)
		{
			Address = addr;
			MatchingStat = (GameStat)iCounter;
			Key = RemoteMemoryObject.Cache.StringCache.Read($"{"StatsDat"}{addr}", () => m.ReadStringU(m.Read<long>(addr), 512));
			Flag0 = m.Read<byte>(addr + 8) != 0;
			IsLocal = m.Read<byte>(addr + 9) != 0;
			IsWeaponLocal = m.Read<byte>(addr + 10) != 0;
			Type = (Key.Contains("%") ? StatType.Percents : ((StatType)m.Read<int>(addr + 11)));
			Flag3 = m.Read<byte>(addr + 15) != 0;
			UserFriendlyName = RemoteMemoryObject.Cache.StringCache.Read($"{"StatsDat"}{addr + 16}", () => m.ReadStringU(m.Read<long>(addr + 16), 512));
			ID = iCounter;
		}

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(UserFriendlyName))
			{
				return UserFriendlyName;
			}
			return Key;
		}

		public string ValueToString(int val)
		{
			switch (Type)
			{
			case StatType.Boolean:
				if (val == 0)
				{
					return "False";
				}
				return "True";
			case StatType.Value2:
			case StatType.IntValue:
				return val.ToString("+#;-#");
			case StatType.Percents:
			case StatType.Precents5:
				return val.ToString("+#;-#") + "%";
			default:
				return "";
			}
		}
	}

	private readonly Dictionary<long, StatRecord> _recordsByAddress = new Dictionary<long, StatRecord>();

	public IDictionary<string, StatRecord> records { get; } = new Dictionary<string, StatRecord>(StringComparer.OrdinalIgnoreCase);


	public IDictionary<int, StatRecord> recordsById { get; } = new Dictionary<int, StatRecord>();


	public StatsDat(IMemory m, Func<long> address)
		: base(m, address)
	{
		loadItems();
	}

	public StatRecord GetStatByAddress(long address)
	{
		return _recordsByAddress.GetValueOrDefault(address);
	}

	private void loadItems()
	{
		int num = 1;
		foreach (long item in RecordAddresses())
		{
			StatRecord statRecord = new StatRecord(base.M, item, num++);
			records[statRecord.Key] = statRecord;
			recordsById[statRecord.ID] = statRecord;
			_recordsByAddress[statRecord.Address] = statRecord;
		}
	}
}
