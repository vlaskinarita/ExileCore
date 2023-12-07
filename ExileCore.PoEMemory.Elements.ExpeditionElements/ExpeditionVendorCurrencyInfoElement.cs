using System.Text.RegularExpressions;

namespace ExileCore.PoEMemory.Elements.ExpeditionElements;

public class ExpeditionVendorCurrencyInfoElement : Element
{
	private static readonly Regex NonNumberRegex = new Regex("[^\\d]");

	public int GwennenRerolls => ParseAmountText(GetChildFromIndices(0, 1)?.Text);

	public int TujenRerolls => ParseAmountText(GetChildFromIndices(1, 1)?.Text);

	public int RogRerolls => ParseAmountText(GetChildFromIndices(2, 1)?.Text);

	public int DannigRerolls => ParseAmountText(GetChildFromIndices(3, 1)?.Text);

	private static int ParseAmountText(string text)
	{
		if (!int.TryParse(NonNumberRegex.Replace(text ?? string.Empty, string.Empty), out var result))
		{
			return 0;
		}
		return result;
	}
}
