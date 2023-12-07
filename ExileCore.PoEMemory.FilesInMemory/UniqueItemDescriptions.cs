using System;
using System.Collections.Generic;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class UniqueItemDescriptions : UniversalFileWrapper<UniqueItemDescription>
{
	private readonly Dictionary<ItemVisualIdentity, List<UniqueItemDescription>> _visualIdentityDictionary = new Dictionary<ItemVisualIdentity, List<UniqueItemDescription>>();

	public UniqueItemDescriptions(IMemory mem, Func<long> address)
		: base(mem, address)
	{
	}

	protected override void EntryAdded(long addr, UniqueItemDescription entry)
	{
		if (addr == 0L)
		{
			base.EntriesList.Remove(entry);
			base.EntriesAddressDictionary.Remove(0L);
		}
		else if (entry.ItemVisualIdentity != null)
		{
			if (!_visualIdentityDictionary.TryGetValue(entry.ItemVisualIdentity, out var value))
			{
				value = (_visualIdentityDictionary[entry.ItemVisualIdentity] = new List<UniqueItemDescription>());
			}
			value.Add(entry);
		}
	}

	public List<UniqueItemDescription> GetByVisualIdentity(ItemVisualIdentity itemVisualIdentity)
	{
		CheckCache();
		return _visualIdentityDictionary.GetValueOrDefault(itemVisualIdentity) ?? new List<UniqueItemDescription>();
	}
}
