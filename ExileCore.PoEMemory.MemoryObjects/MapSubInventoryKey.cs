using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.MemoryObjects;

public class MapSubInventoryKey
{
	public string Path { get; set; }

	public MapType Type { get; set; }

	public override string ToString()
	{
		return $"Path:{Path} Type:{Type}";
	}
}
