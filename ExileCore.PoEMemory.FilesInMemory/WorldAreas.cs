using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class WorldAreas : UniversalFileWrapper<WorldArea>
{
	private int _indexCounter;

	public Dictionary<int, WorldArea> AreasIndexDictionary { get; } = new Dictionary<int, WorldArea>();


	public Dictionary<int, WorldArea> AreasWorldIdDictionary { get; } = new Dictionary<int, WorldArea>();


	public WorldAreas(IMemory m, Func<long> address)
		: base(m, address)
	{
	}

	public WorldArea GetAreaByAreaId(int index)
	{
		CheckCache();
		AreasIndexDictionary.TryGetValue(index, out var value);
		return value;
	}

	public WorldArea GetAreaByAreaId(string id)
	{
		CheckCache();
		return AreasIndexDictionary.First((KeyValuePair<int, WorldArea> area) => area.Value.Id == id).Value;
	}

	public WorldArea GetAreaByWorldId(int id)
	{
		CheckCache();
		AreasWorldIdDictionary.TryGetValue(id, out var value);
		return value;
	}

	protected override void EntryAdded(long addr, WorldArea entry)
	{
		entry.Index = _indexCounter++;
		AreasIndexDictionary.Add(entry.Index, entry);
		AreasWorldIdDictionary.Add(entry.WorldAreaId, entry);
	}
}
