namespace ExileCore.PoEMemory.Components;

public class Magnetic : Component
{
	public int Force => base.M.Read<int>(base.Address + 48);
}
