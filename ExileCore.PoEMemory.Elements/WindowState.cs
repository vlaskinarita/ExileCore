namespace ExileCore.PoEMemory.Elements;

public class WindowState : Element
{
	public new bool IsVisibleLocal => base.M.Read<int>(base.Address + 2144) == 1;
}
