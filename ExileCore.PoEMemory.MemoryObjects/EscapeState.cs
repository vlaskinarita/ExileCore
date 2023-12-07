namespace ExileCore.PoEMemory.MemoryObjects;

public class EscapeState : GameState
{
	public const int IsActiveOffset = 416;

	public bool IsActive => base.M.Read<bool>(base.Address + 416);
}
