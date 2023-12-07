using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.MemoryObjects;

public class QuestRewardWindow : Element
{
	public Element CancelButton => GetChildAtIndex(3);

	public Element SelectOneRewardString => GetChildAtIndex(0);

	public IList<(Entity, Element)> GetPossibleRewards()
	{
		List<Element> obj = GetChildFromIndices(5, 0)?.Children.SelectMany((Element x) => x.Children).ToList();
		List<(Entity, Element)> list = new List<(Entity, Element)>();
		foreach (Element item2 in obj)
		{
			Entity item = item2.GetChildFromIndices(0, 1)?.Entity;
			list.Add((item, item2));
		}
		return list;
	}
}
