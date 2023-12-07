namespace ExileCore.PoEMemory.Elements.InventoryElements;

public class MapSubInventoryKey
{
	public string Path;

	public MapType Type;

	public override string ToString()
	{
		return $"Path:{Path} Type:{Type}";
	}
}
