using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.MemoryObjects;

public class GemLvlUpPanel : Element
{
	public IList<Element> GemsToLvlUp => GetChildAtIndex(0)?.Children;

	public List<(Entity, Element)> Gems => GemsToLvlUp.Select((Element gem) => (gem?.ReadObject<Entity>(gem.Address + 496), gem)).ToList();

	public Element LvlUpButtonForGem(Element gem)
	{
		return gem?.GetChildAtIndex(1);
	}

	public bool MeetRequirementForGem(Element gem)
	{
		string text = TextForGem(gem)?.Text;
		if (text == null)
		{
			return false;
		}
		return text.ToLower().Trim() == "click to level up";
	}

	public Element TextForGem(Element gem)
	{
		return gem?.GetChildAtIndex(3);
	}
}
