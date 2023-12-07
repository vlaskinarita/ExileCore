namespace ExileCore.PoEMemory.Components;

public class Monolith : Component
{
	public int OpenStage => base.M.Read<byte>(base.Address + 112);

	public bool IsOpened => OpenStage == 4;
}
