using System;
using System.Collections.Generic;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.MemoryObjects;

public class InventoryList : RemoteMemoryObject
{
	public static int InventoryCount => 53;

	public Inventory this[InventoryIndex inv]
	{
		get
		{
			if (inv < InventoryIndex.None || (int)inv >= InventoryCount)
			{
				return null;
			}
			return ReadObjectAt<PlayerInventory>((int)inv * 8);
		}
	}

	public List<Inventory> DebugInventories => _debug();

	private List<Inventory> _debug()
	{
		List<Inventory> list = new List<Inventory>();
		InventoryIndex[] values = Enum.GetValues<InventoryIndex>();
		for (int i = 0; i < values.Length; i++)
		{
			int num = (int)values[i];
			if (num < 0 || num >= InventoryCount)
			{
				return null;
			}
			list.Add(ReadObjectAt<PlayerInventory>(num * 8));
		}
		return list;
	}
}
