using System;
using System.Collections.Generic;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class ItemVisualIdentities : UniversalFileWrapper<ItemVisualIdentity>
{
	private readonly Dictionary<string, List<ItemVisualIdentity>> _artPathDictionary = new Dictionary<string, List<ItemVisualIdentity>>();

	public ItemVisualIdentities(IMemory mem, Func<long> address)
		: base(mem, address)
	{
	}

	protected override void EntryAdded(long addr, ItemVisualIdentity entry)
	{
		if (!_artPathDictionary.TryGetValue(entry.ArtPath, out var value))
		{
			value = (_artPathDictionary[entry.ArtPath] = new List<ItemVisualIdentity>());
		}
		value.Add(entry);
	}

	public List<ItemVisualIdentity> GetByArtPath(string artPath)
	{
		CheckCache();
		return _artPathDictionary.GetValueOrDefault(artPath) ?? new List<ItemVisualIdentity>();
	}
}
