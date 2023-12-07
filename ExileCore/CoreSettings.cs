using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ClickableTransparentOverlay;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using Newtonsoft.Json;

namespace ExileCore;

public class CoreSettings : ISettings
{
	[Menu("Refresh area")]
	[JsonIgnore]
	public ButtonNode RefreshArea { get; set; } = new ButtonNode();


	[JsonIgnore]
	public ButtonNode ReloadFiles { get; set; } = new ButtonNode();


	[Menu(null, "Uses more memory, should be faster")]
	public ToggleNode UseNewMemoryBackend { get; set; } = new ToggleNode(value: true);


	[Menu("List profiles", "Currently not works. Soon.")]
	public ListNode Profiles { get; set; } = new ListNode
	{
		Values = new List<string> { "global" },
		Value = "global"
	};


	[Menu("Current Menu Theme")]
	public ListNode Theme { get; set; } = new ListNode
	{
		Value = "Default"
	};


	public HotkeyNode MainMenuKeyToggle { get; set; } = Keys.F12;


	public CorePluginSettings PluginSettings { get; set; } = new CorePluginSettings();


	public CorePerformanceSettings PerformanceSettings { get; set; } = new CorePerformanceSettings();


	[Menu("Enable VSync")]
	public ToggleNode EnableVSync { get; set; } = new ToggleNode(value: false);


	public ListNode Font { get; set; } = new ListNode
	{
		Values = new List<string> { "Not found" }
	};


	public ListNode FontGlyphRange { get; set; } = new ListNode
	{
		Values = (from x in Enum.GetValues<FontGlyphRangeType>()
			select x.ToString()).ToList(),
		Value = FontGlyphRangeType.Cyrillic.ToString()
	};


	[Menu("Font size", "Currently not works. Because this option broke calculate how much pixels needs for render.")]
	[IgnoreMenu]
	public RangeNode<int> FontSize { get; set; } = new RangeNode<int>(13, 7, 36);


	[Menu(null, "If unchecked, some plugin may ignore the selected font")]
	public ToggleNode ApplySelectedFontGlobally { get; set; } = new ToggleNode(value: true);


	[Menu(null, "If you use a large custom cursor, this can help see tooltips better")]
	public RangeNode<float> MouseCursorScale { get; set; } = new RangeNode<float>(1f, 0.1f, 10f);


	public RangeNode<int> Volume { get; set; } = new RangeNode<int>(100, 0, 100);


	public ToggleNode ShowDebugWindow { get; set; } = new ToggleNode(value: false);


	public ToggleNode ShowLogWindow { get; set; } = new ToggleNode(value: false);


	public ToggleNode ShowDemoWindow { get; set; } = new ToggleNode(value: false);


	public ToggleNode Enable { get; set; } = new ToggleNode(value: true);


	public ToggleNode ForceForeground { get; set; } = new ToggleNode(value: false);


	public ToggleNode HideAllDebugging { get; set; } = new ToggleNode(value: false);


	[Menu(null, "If you have a widescreen display and use some sort of a hack to force poe to render without black bars, you might need this")]
	public ToggleNode DisableBlackBarAdjustment { get; set; } = new ToggleNode(value: false);


	[JsonIgnore]
	public ButtonNode ReloadPlugins { get; set; } = new ButtonNode();

}
