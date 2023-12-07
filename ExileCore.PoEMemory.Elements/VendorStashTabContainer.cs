using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements;

public class VendorStashTabContainer : StashTabContainer
{
	public override VendorInventory GetStashInventoryByIndex(int index)
	{
		return base.GetStashInventoryByIndex(index)?.AsObject<VendorInventory>();
	}
}
