using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Components;

public class Prophecy : Component
{
	public ProphecyDat DatProphecy => base.TheGame.Files.Prophecies.GetByAddress(base.M.Read<long>(base.Address + 24));
}
