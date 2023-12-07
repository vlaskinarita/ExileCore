namespace ExileCore.PoEMemory.Elements;

public class TreePanel : Element
{
	private const int CanvasElementOffset = 1024;

	public Element CanvasElement => ReadObjectAt<Element>(1024);
}
