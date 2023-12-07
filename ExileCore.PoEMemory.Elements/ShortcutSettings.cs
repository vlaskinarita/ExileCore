using System.Collections.Generic;
using System.Linq;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Elements;

public class ShortcutSettings : Element
{
	public StdVector ShortcutArray => base.M.Read<StdVector>(base.Address + 760);

	public IList<Shortcut> Shortcuts => base.M.ReadStdVector<Shortcut>(ShortcutArray);

	public Shortcut LeagueInterfaceShortcut => Shortcuts.FirstOrDefault((Shortcut x) => x.Usage == ShortcutUsage.LeagueInterface);

	public Shortcut LeaguePanelShortcut => Shortcuts.FirstOrDefault((Shortcut x) => x.Usage == ShortcutUsage.LeaguePanel);

	public Shortcut StalkerSentinelShortcut => Shortcuts.FirstOrDefault((Shortcut x) => x.Usage == ShortcutUsage.StalkerSentinel);

	public Shortcut PandemoniumSentinelShortcut => Shortcuts.FirstOrDefault((Shortcut x) => x.Usage == ShortcutUsage.PandemoniumSentinel);

	public Shortcut ApexSentinelShortcut => Shortcuts.FirstOrDefault((Shortcut x) => x.Usage == ShortcutUsage.ApexSentinel);
}
