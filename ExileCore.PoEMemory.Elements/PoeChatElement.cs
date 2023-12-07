using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Elements;

public class PoeChatElement : Element
{
	public long TotalMessageCount => base.ChildCount;

	public new EntityLabel this[int index]
	{
		get
		{
			if (index < TotalMessageCount)
			{
				return GetChildAtIndex(index).AsObject<EntityLabel>();
			}
			return null;
		}
	}

	public List<Element> MessageElements => GetChildrenAs<Element>();

	public List<string> Messages => MessageElements.Select((Element x) => x.Text).ToList();
}
