namespace ExileCore.PoEMemory.Elements.InventoryElements;

public class MapSubInventoryInfo
{
	public int Count;

	public string MapName;

	public int Tier;

	public override string ToString()
	{
		return $"Tier:{Tier} Count:{Count} MapName:{MapName}";
	}
}
