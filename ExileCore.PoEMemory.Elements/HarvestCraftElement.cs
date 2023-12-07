using System.Text.RegularExpressions;

namespace ExileCore.PoEMemory.Elements;

public class HarvestCraftElement : Element
{
	private static readonly Regex _replaceRegex = new Regex("\\<\\w+\\>|\\{|\\}");

	public string CraftDisplayName => _replaceRegex.Replace(GetText(2000), string.Empty);
}
