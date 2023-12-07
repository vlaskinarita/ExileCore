using System;
using System.Collections.Generic;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.Elements;

public class InventoryElement : Element
{
	private InventoryList _allInventories;

	private InventoryList AllInventories
	{
		get
		{
			InventoryList obj = _allInventories ?? GetObjectAt<InventoryList>(872);
			InventoryList result = obj;
			_allInventories = obj;
			return result;
		}
	}

	public Inventory this[InventoryIndex k] => AllInventories[k];

	private Element EquippedItems => GetChildAtIndex(3);

	public IList<Element> GetItemsInInventory()
	{
		IList<Element> children = GetElementSlot(InventoryIndex.PlayerInventory).Children;
		children.RemoveAt(0);
		return children;
	}

	public Element GetElementSlot(InventoryIndex inventoryIndex)
	{
		return inventoryIndex switch
		{
			InventoryIndex.None => throw new ArgumentOutOfRangeException("inventoryIndex"), 
			InventoryIndex.Helm => EquippedItems.GetChildAtIndex(12), 
			InventoryIndex.Amulet => EquippedItems.GetChildAtIndex(13), 
			InventoryIndex.Chest => EquippedItems.GetChildAtIndex(19), 
			InventoryIndex.LWeapon => EquippedItems.GetChildAtIndex(16), 
			InventoryIndex.RWeapon => EquippedItems.GetChildAtIndex(15), 
			InventoryIndex.LWeaponSwap => EquippedItems.GetChildAtIndex(18), 
			InventoryIndex.RWeaponSwap => EquippedItems.GetChildAtIndex(17), 
			InventoryIndex.LRing => EquippedItems.GetChildAtIndex(20), 
			InventoryIndex.RRing => EquippedItems.GetChildAtIndex(21), 
			InventoryIndex.Gloves => EquippedItems.GetChildAtIndex(22), 
			InventoryIndex.Belt => EquippedItems.GetChildAtIndex(23), 
			InventoryIndex.Boots => EquippedItems.GetChildAtIndex(24), 
			InventoryIndex.PlayerInventory => EquippedItems.GetChildAtIndex(26), 
			InventoryIndex.Flask => EquippedItems.GetChildAtIndex(25), 
			_ => throw new ArgumentOutOfRangeException("inventoryIndex"), 
		};
	}
}
