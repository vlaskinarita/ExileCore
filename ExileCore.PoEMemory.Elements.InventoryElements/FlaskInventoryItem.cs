namespace ExileCore.PoEMemory.Elements.InventoryElements;

public class FlaskInventoryItem : NormalInventoryItem
{
	public override int InventPosX => 0;

	public override int InventPosY => 0;

	public override RectangleF GetClientRect()
	{
		return base.Parent?.GetClientRect() ?? RectangleF.Empty;
	}
}
