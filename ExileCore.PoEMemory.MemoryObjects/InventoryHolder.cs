using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.MemoryObjects;

public class InventoryHolder : RemoteMemoryObject
{
	public const int StructSize = 32;

	public int Id => base.M.Read<int>(base.Address);

	public InventoryNameE TypeId => (InventoryNameE)Id;

	public ServerInventory Inventory => ReadObject<ServerInventory>(base.Address + 8);

	public override string ToString()
	{
		return $"InventoryType: {Inventory.InventType}, InventorySlot: {Inventory.InventSlot}, Items.Count: {Inventory.Items.Count} ItemCount: {Inventory.ItemCount}";
	}
}
