namespace ExileCore.PoEMemory.Elements;

public class SyndicatePanel : Element
{
	public Element EventElement
	{
		get
		{
			Element childAtIndex = GetChildAtIndex(0);
			if (childAtIndex.ChildCount < 25)
			{
				return null;
			}
			Element childAtIndex2 = childAtIndex.GetChildAtIndex(24);
			if (childAtIndex2.GetChildFromIndices(8, 1) == null)
			{
				return childAtIndex.GetChildAtIndex(25);
			}
			return childAtIndex2;
		}
	}

	public Element TextElement => EventElement?.GetChildFromIndices(8, 1);

	public string EventText => TextElement.Text;
}
