using SharpDX;

namespace ExileCore.PoEMemory.Elements.InventoryElements;

public class DivinationInventoryItem : NormalInventoryItem
{
	public override int InventPosX => 0;

	public override int InventPosY => 0;

	public override RectangleF GetClientRect()
	{
		RectangleF rectangleF = base.Parent?.GetClientRect() ?? RectangleF.Empty;
		if (rectangleF == RectangleF.Empty)
		{
			return rectangleF;
		}
		long? num = base.Parent?.Parent?.Parent?.Parent?[2].Address;
		if (!num.HasValue)
		{
			return rectangleF;
		}
		float num2 = (float)base.M.Read<int>(num.Value + 2660) * 107.5f;
		rectangleF.Y -= num2;
		return rectangleF;
	}
}
