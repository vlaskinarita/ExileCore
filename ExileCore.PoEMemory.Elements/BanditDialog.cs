using System;

namespace ExileCore.PoEMemory.Elements;

public class BanditDialog : Element
{
	public Element HelpButton => GetChildAtIndex(2)?.GetChildAtIndex(0);

	public Element KillButton => GetChildAtIndex(2)?.GetChildAtIndex(1);

	public BanditType BanditType => GetBanditType();

	private BanditType GetBanditType()
	{
		if (HelpButton == null)
		{
			DebugWindow.LogError("BanditDialog.HelpButton is null, either window is not open or check offsets");
		}
		string text = HelpButton?.GetChildAtIndex(0)?.Text?.ToLower();
		if (text == null)
		{
			throw new ArgumentException();
		}
		if (text.Contains("kraityn"))
		{
			return BanditType.Kraityn;
		}
		if (text.Contains("alira"))
		{
			return BanditType.Alira;
		}
		if (text.Contains("oak"))
		{
			return BanditType.Oak;
		}
		throw new ArgumentException();
	}
}
