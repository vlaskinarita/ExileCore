using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Components;

public class Animated : Component
{
	private const int BaseAnimatedObjectEntityOffset = 560;

	public Entity BaseAnimatedObjectEntity => GetObject<Entity>(base.M.Read<long>(base.Address + 560));
}
