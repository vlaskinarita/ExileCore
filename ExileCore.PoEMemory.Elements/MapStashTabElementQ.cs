using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Elements;

public class MapStashTabElementQ : Element
{
	public Dictionary<string, string> MapsCount => GetMapsCount();

	public Dictionary<string, string> CurrentCell => GetCurrentCell();

	private Dictionary<string, string> GetCurrentCell()
	{
		IList<Element> children = base.Children[2].Children[0].Children[0].Children;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (Element item in children)
		{
			string text = item?.Tooltip?.Children?[0].Children[0].Children[3].Text;
			if (text == null)
			{
				string text2 = item.Tooltip?.Text;
				text = ((text2 != null) ? text2.Substring(0, text2.IndexOf('\n')) : "Error");
			}
			string text3 = item.Children[4].Text;
			dictionary.Add(text, text3);
		}
		return dictionary;
	}

	private Dictionary<string, string> GetMapsCount()
	{
		IEnumerable<Element> enumerable = base.Children[0].Children.Concat(base.Children[1].Children);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (Element item in enumerable)
		{
			dictionary.Add(item.Children[0].Text, item.Children[1].Text);
		}
		return dictionary;
	}
}
