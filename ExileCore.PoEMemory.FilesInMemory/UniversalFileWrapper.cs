using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class UniversalFileWrapper<RecordType> : FileInMemory where RecordType : RemoteMemoryObject, new()
{
	public bool ExcludeZeroAddresses { get; set; }

	protected Dictionary<long, RecordType> EntriesAddressDictionary { get; set; } = new Dictionary<long, RecordType>();


	protected List<RecordType> CachedEntriesList { get; set; } = new List<RecordType>();


	public List<RecordType> EntriesList
	{
		get
		{
			CheckCache();
			return CachedEntriesList;
		}
	}

	public UniversalFileWrapper(IMemory mem, Func<long> address)
		: base(mem, address)
	{
	}

	public RecordType GetByAddress(long address)
	{
		CheckCache();
		EntriesAddressDictionary.TryGetValue(address, out var value);
		return value;
	}

	public void CheckCache()
	{
		if (EntriesAddressDictionary.Count != 0)
		{
			return;
		}
		foreach (long item in from x in RecordAddresses()
			where !ExcludeZeroAddresses || x != 0
			select x)
		{
			if (!EntriesAddressDictionary.ContainsKey(item))
			{
				RecordType @object = RemoteMemoryObject.pTheGame.GetObject<RecordType>(item);
				EntriesAddressDictionary.Add(item, @object);
				EntriesList.Add(@object);
				EntryAdded(item, @object);
			}
		}
	}

	protected virtual void EntryAdded(long addr, RecordType entry)
	{
	}
}
