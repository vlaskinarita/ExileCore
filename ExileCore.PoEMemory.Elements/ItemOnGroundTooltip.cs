namespace ExileCore.PoEMemory.Elements;

public class ItemOnGroundTooltip : Element
{
	public Element ItemFrame => TooltipUI?.GetChildAtIndex(1);

	public Element TooltipUI => GetChildAtIndex(0)?.GetChildAtIndex(0);

	public Element Item2DIcon => TooltipUI?.GetChildAtIndex(0);

	public new Element Tooltip => TooltipUI;
}
