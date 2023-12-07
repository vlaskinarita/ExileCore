using System.Linq;

namespace ExileCore.PoEMemory.MemoryObjects.Ancestor;

public class AncestorFightSelectionOpponentLine : Element
{
	public Entity Reward => RewardElement?.Children.LastOrDefault()?.ReadObjectAt<Entity>(896);

	public Element RewardElement => GetChildFromIndices(4, 0);
}
