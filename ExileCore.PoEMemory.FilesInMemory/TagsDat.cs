using System;
using System.Collections.Generic;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class TagsDat : FileInMemory
{
	public class TagRecord
	{
		public string Key { get; }

		public int Hash { get; }

		public TagRecord(IMemory m, long addr)
		{
			Key = RemoteMemoryObject.Cache.StringCache.Read($"{"TagsDat"}{addr}", () => m.ReadStringU(m.Read<long>(addr), 255));
			Hash = m.Read<int>(addr + 8);
		}
	}

	public Dictionary<string, TagRecord> Records { get; } = new Dictionary<string, TagRecord>(StringComparer.OrdinalIgnoreCase);


	public TagsDat(IMemory m, Func<long> address)
		: base(m, address)
	{
		loadItems();
	}

	private void loadItems()
	{
		foreach (long item in RecordAddresses())
		{
			TagRecord tagRecord = new TagRecord(base.M, item);
			if (!Records.ContainsKey(tagRecord.Key))
			{
				Records.Add(tagRecord.Key, tagRecord);
			}
		}
	}
}
