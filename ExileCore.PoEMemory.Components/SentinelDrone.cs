namespace ExileCore.PoEMemory.Components;

public class SentinelDrone : Component
{
	public int TimesUsed => base.M.Read<int>(base.Address + 32);

	public int MaxUses => base.M.Read<int>(base.Address + 16, new int[2] { 16, 32 });
}
