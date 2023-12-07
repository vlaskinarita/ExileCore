namespace ExileCore.PoEMemory.Components;

public class ClientAnimationController : Component
{
	public int AnimKey => base.M.Read<int>(base.Address + 156);
}
