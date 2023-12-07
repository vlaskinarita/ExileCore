using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory.Atlas;

public class AtlasNodes : UniversalFileWrapper<AtlasNode>
{
	public new IList<AtlasNode> EntriesList
	{
		get
		{
			CheckCache();
			return base.CachedEntriesList.ToList();
		}
	}

	public AtlasNodes(IMemory mem, Func<long> address)
		: base(mem, address)
	{
	}

	public new AtlasNode GetByAddress(long address)
	{
		return base.GetByAddress(address);
	}
}
