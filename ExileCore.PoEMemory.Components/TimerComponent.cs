namespace ExileCore.PoEMemory.Components;

public class TimerComponent : Component
{
	public float TimeLeft => base.M.Read<float>(base.Address + 24);
}
