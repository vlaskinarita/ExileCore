using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class Quests : UniversalFileWrapper<Quest>
{
	public new IList<Quest> EntriesList
	{
		get
		{
			CheckCache();
			return base.CachedEntriesList.ToList();
		}
	}

	public Quests(IMemory game, Func<long> address)
		: base(game, address)
	{
	}

	public new Quest GetByAddress(long address)
	{
		return base.GetByAddress(address);
	}
}
