namespace ExileCore.PoEMemory.MemoryObjects;

public class MapSubInventoryInfo
{
	public int Tier { get; set; }

	public int Count { get; set; }

	public string MapName { get; set; }

	public override string ToString()
	{
		return $"Tier:{Tier} Count:{Count} MapName:{MapName}";
	}
}
