using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class PropheciesDat : UniversalFileWrapper<ProphecyDat>
{
	private int IndexCounter;

	private bool loaded;

	private readonly Dictionary<int, ProphecyDat> ProphecyIndexDictionary = new Dictionary<int, ProphecyDat>();

	public new IList<ProphecyDat> EntriesList => base.EntriesList.ToList();

	public PropheciesDat(IMemory m, Func<long> address)
		: base(m, address)
	{
	}

	public ProphecyDat GetProphecyById(int index)
	{
		CheckCache();
		if (!loaded)
		{
			foreach (ProphecyDat entries in EntriesList)
			{
				EntryAdded(entries.Address, entries);
			}
			loaded = true;
		}
		ProphecyIndexDictionary.TryGetValue(index, out var value);
		return value;
	}

	protected new void EntryAdded(long addr, ProphecyDat entry)
	{
		entry.Index = IndexCounter++;
		ProphecyIndexDictionary.Add(entry.ProphecyId, entry);
	}

	public new ProphecyDat GetByAddress(long address)
	{
		return base.GetByAddress(address);
	}
}
