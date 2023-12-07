using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements;

public class SentinelSubPanel : Element
{
	public SentinelData SentinelData => base.Tooltip.ReadObjectAt<SentinelData>(1464);

	public Entity SentinelItem => GetChildFromIndices(default(int), default(int))?.ReadObjectAt<Entity>(896);
}
