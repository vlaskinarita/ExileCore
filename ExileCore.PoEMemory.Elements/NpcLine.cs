using System;

namespace ExileCore.PoEMemory.Elements;

public class NpcLine
{
	public Element Element { get; }

	public string Text { get; }

	public NpcLine(Element element)
	{
		Element = element ?? throw new ArgumentNullException("element");
		Text = Element.GetChildAtIndex(0)?.Text ?? throw new ArgumentOutOfRangeException("element");
	}

	public override string ToString()
	{
		return Text;
	}
}
