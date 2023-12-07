using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Components;

public class Portal : Component
{
	public WorldArea Area => base.TheGame.Files.WorldAreas.GetByAddress(base.M.Read<long>(base.Address + 48));
}
