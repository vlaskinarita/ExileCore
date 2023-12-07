using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Components;

public class HeistRewardDisplay : Component
{
	public Entity RewardItem => ReadObjectAt<Entity>(32);
}
