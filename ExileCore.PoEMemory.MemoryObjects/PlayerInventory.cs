using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.MemoryObjects;

public class PlayerInventory : Inventory
{
	protected override Element OffsetContainerElement => this;

	protected override InventoryType GetInvType()
	{
		return InventoryType.PlayerInventory;
	}
}
