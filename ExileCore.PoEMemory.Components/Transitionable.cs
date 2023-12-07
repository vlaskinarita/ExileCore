namespace ExileCore.PoEMemory.Components;

public class Transitionable : Component
{
	public byte Flag1 => base.M.Read<byte>(base.Address + 288);

	public byte Flag2 => base.M.Read<byte>(base.Address + 292);
}
