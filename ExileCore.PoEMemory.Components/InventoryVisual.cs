namespace ExileCore.PoEMemory.Components;

public class InventoryVisual : RemoteMemoryObject
{
	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Texture => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public string Model => base.M.ReadStringU(base.M.Read<long>(base.Address + 16));
}
