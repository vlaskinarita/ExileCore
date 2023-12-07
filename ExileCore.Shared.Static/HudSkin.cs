using SharpDX;

namespace ExileCore.Shared.Static;

public static class HudSkin
{
	public static readonly Color CurrencyColor = new ColorBGRA(170, 158, 130, byte.MaxValue);

	public static readonly Color MagicColor = new ColorBGRA(136, 136, byte.MaxValue, byte.MaxValue);

	public static readonly Color RareColor = new ColorBGRA(byte.MaxValue, byte.MaxValue, 119, byte.MaxValue);

	public static readonly Color UniqueColor = new ColorBGRA(175, 96, 37, byte.MaxValue);

	public static readonly Color DivinationCardColor = new ColorBGRA(220, 0, 0, byte.MaxValue);

	public static readonly Color TalismanColor = new ColorBGRA(208, 31, 144, byte.MaxValue);

	public static readonly Color SkillGemColor = new ColorBGRA(26, 162, 155, byte.MaxValue);

	public static readonly Color DmgFireColor = new ColorBGRA(150, 0, 0, byte.MaxValue);

	public static readonly Color DmgColdColor = new ColorBGRA(54, 100, 146, byte.MaxValue);

	public static readonly Color DmgLightingColor = new ColorBGRA(byte.MaxValue, 215, 0, byte.MaxValue);

	public static readonly Color DmgChaosColor = new ColorBGRA(208, 31, 144, byte.MaxValue);
}
