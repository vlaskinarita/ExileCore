using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;

namespace ExileCore.RenderQ;

public class ThemeEditor
{
	public const string ThemeExtension = ".hudtheme";

	public const string DefaultThemeName = "Default";

	private const string ThemesFolder = "config/themes";

	private readonly CoreSettings coreSettings;

	private ThemeConfig LoadedTheme;

	private string NewThemeName = "MyNewTheme";

	private int SelectedThemeId;

	private string SelectedThemeName;

	public ThemeEditor(CoreSettings coreSettings)
	{
		this.coreSettings = coreSettings;
		GenerateDefaultTheme();
		if (!Directory.Exists("config/themes"))
		{
			Directory.CreateDirectory("config/themes");
			SaveTheme(GenerateDefaultTheme(), "Default");
			coreSettings.Theme.Value = "Default";
		}
		LoadThemeFilesList();
		SelectedThemeName = coreSettings.Theme.Value ?? coreSettings.Theme.Values.FirstOrDefault();
		ApplyTheme(SelectedThemeName);
		ListNode theme = coreSettings.Theme;
		theme.OnValueSelected = (Action<string>)Delegate.Combine(theme.OnValueSelected, new Action<string>(ApplyTheme));
	}

	private void LoadThemeFilesList()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo("config/themes");
		coreSettings.Theme.Values = (from x in directoryInfo.GetFiles("*.hudtheme")
			orderby x.LastWriteTime descending
			select Path.GetFileNameWithoutExtension(x.Name)).ToList();
	}

	public unsafe void DrawSettingsMenu()
	{
		if (ImGui.Combo("Select Theme", ref SelectedThemeId, coreSettings.Theme.Values.ToArray(), coreSettings.Theme.Values.Count) && SelectedThemeName != coreSettings.Theme.Values[SelectedThemeId])
		{
			SelectedThemeName = coreSettings.Theme.Values[SelectedThemeId];
			LoadedTheme = LoadTheme(coreSettings.Theme.Values[SelectedThemeId], nullIfNotFound: false);
			ApplyTheme(LoadedTheme);
		}
		if (ImGui.Button("Save current theme settings to selected"))
		{
			SaveTheme(ReadThemeFromCurrent(), SelectedThemeName);
		}
		ImGui.Text("");
		ImGui.InputText("New theme name", ref NewThemeName, 200u, ImGuiInputTextFlags.None);
		if (ImGui.Button("Create new theme from current") && !string.IsNullOrEmpty(NewThemeName))
		{
			string str = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			Regex regex = new Regex("[" + Regex.Escape(str) + "]");
			NewThemeName = regex.Replace(NewThemeName, "");
			SaveTheme(ReadThemeFromCurrent(), NewThemeName);
			SelectedThemeName = NewThemeName;
			LoadThemeFilesList();
		}
		ImGui.Text("");
		ImGuiStylePtr style = ImGui.GetStyle();
		if (ImGui.TreeNode("Theme settings"))
		{
			style.AntiAliasedFill = DrawBoolSetting("AntiAliasedFill", style.AntiAliasedFill);
			style.AntiAliasedLines = DrawBoolSetting("AntiAliasedLines", style.AntiAliasedLines);
			style.Alpha = DrawFloatSetting("Alpha", style.Alpha * 100f, 0f, 100f) / 100f;
			style.DisplayWindowPadding = DrawVectorSetting("DisplayWindowPadding", style.DisplayWindowPadding, 0f, 20f);
			style.TouchExtraPadding = DrawVectorSetting("TouchExtraPadding", style.TouchExtraPadding, 0f, 10f);
			style.WindowPadding = DrawVectorSetting("WindowPadding", style.WindowPadding, 0f, 20f);
			style.FramePadding = DrawVectorSetting("FramePadding", style.FramePadding, 0f, 20f);
			style.DisplaySafeAreaPadding = DrawVectorSetting("DisplaySafeAreaPadding", style.DisplaySafeAreaPadding, 0f, 20f);
			style.ItemInnerSpacing = DrawVectorSetting("ItemInnerSpacing", style.ItemInnerSpacing, 0f, 20f);
			style.ItemSpacing = DrawVectorSetting("ItemSpacing", style.ItemSpacing, 0f, 20f);
			style.GrabMinSize = DrawFloatSetting("GrabMinSize", style.GrabMinSize, 0f, 20f);
			style.GrabRounding = DrawFloatSetting("GrabRounding", style.GrabRounding, 0f, 12f);
			style.IndentSpacing = DrawFloatSetting("IndentSpacing", style.IndentSpacing, 0f, 30f);
			style.ScrollbarRounding = DrawFloatSetting("ScrollbarRounding", style.ScrollbarRounding, 0f, 19f);
			style.ScrollbarSize = DrawFloatSetting("ScrollbarSize", style.ScrollbarSize, 0f, 20f);
			style.WindowTitleAlign = DrawVectorSetting("WindowTitleAlign", style.WindowTitleAlign, 0f, 1f);
			style.WindowRounding = DrawFloatSetting("WindowRounding", style.WindowRounding, 0f, 14f);
			style.ChildRounding = DrawFloatSetting("ChildWindowRounding", style.ChildRounding, 0f, 16f);
			style.FrameRounding = DrawFloatSetting("FrameRounding", style.FrameRounding, 0f, 12f);
			style.ColumnsMinSpacing = DrawFloatSetting("ColumnsMinSpacing", style.ColumnsMinSpacing, 0f, 30f);
			style.CurveTessellationTol = DrawFloatSetting("CurveTessellationTolerance", style.CurveTessellationTol * 100f, 0f, 100f) / 100f;
		}
		ImGui.Text("");
		ImGui.Text("Colors:");
		ImGui.Columns(2, "Columns", border: true);
		ImGuiCol[] values = Enum.GetValues<ImGuiCol>();
		int num = values.Length / 2;
		ImGuiCol[] array = values;
		for (int i = 0; i < array.Length; i++)
		{
			ImGuiCol idx = array[i];
			string label = Regex.Replace(idx.ToString(), "(\\B[A-Z])", " $1");
			Vector4* styleColorVec = ImGui.GetStyleColorVec4(idx);
			Vector4 col = new Vector4(styleColorVec->X, styleColorVec->Y, styleColorVec->Z, styleColorVec->W);
			if (ImGui.ColorEdit4(label, ref col, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.AlphaPreviewHalf))
			{
				ImGui.PushStyleColor(idx, col);
			}
			if (num-- == -1)
			{
				ImGui.NextColumn();
			}
		}
	}

	private bool DrawBoolSetting(string name, bool result)
	{
		ImGui.Checkbox(name, ref result);
		return result;
	}

	private float DrawFloatSetting(string name, float result, float min, float max)
	{
		ImGui.SliderFloat(name, ref result, min, max, "%.0f");
		return result;
	}

	private Vector2 DrawVectorSetting(string name, Vector2 result, float min, float max)
	{
		ImGui.SliderFloat2(name, ref result, min, max, "%.0f");
		return result;
	}

	public static void ApplyTheme(string fileName)
	{
		ThemeConfig themeConfig = LoadTheme(fileName, nullIfNotFound: true);
		if (themeConfig == null)
		{
			DebugWindow.LogMsg("Can't find theme file " + fileName + ", loading default.", 3f);
			themeConfig = LoadTheme("Default", nullIfNotFound: true);
			if (themeConfig == null)
			{
				DebugWindow.LogMsg("Can't find default theme file Default, Generating default and saving...", 3f);
				themeConfig = GenerateDefaultTheme();
				SaveTheme(themeConfig, "Default");
			}
		}
		ApplyTheme(themeConfig);
	}

	public static void ApplyTheme(ThemeConfig theme)
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		style.AntiAliasedLines = theme.AntiAliasedLines;
		style.DisplaySafeAreaPadding = theme.DisplaySafeAreaPadding;
		style.DisplayWindowPadding = theme.DisplayWindowPadding;
		style.GrabRounding = theme.GrabRounding;
		style.GrabMinSize = theme.GrabMinSize;
		style.ScrollbarRounding = theme.ScrollbarRounding;
		style.ScrollbarSize = theme.ScrollbarSize;
		style.ColumnsMinSpacing = theme.ColumnsMinSpacing;
		style.IndentSpacing = theme.IndentSpacing;
		style.TouchExtraPadding = theme.TouchExtraPadding;
		style.ItemInnerSpacing = theme.ItemInnerSpacing;
		style.ItemSpacing = theme.ItemSpacing;
		style.FrameRounding = theme.FrameRounding;
		style.FramePadding = theme.FramePadding;
		style.ChildRounding = theme.ChildWindowRounding;
		style.WindowTitleAlign = theme.WindowTitleAlign;
		style.WindowRounding = theme.WindowRounding;
		style.WindowPadding = theme.WindowPadding;
		style.Alpha = theme.Alpha;
		style.AntiAliasedFill = theme.AntiAliasedFill;
		style.CurveTessellationTol = theme.CurveTessellationTolerance;
		foreach (KeyValuePair<ImGuiCol, Vector4> color in theme.Colors)
		{
			try
			{
				if (color.Key != ImGuiCol.COUNT)
				{
					ImGui.PushStyleColor(color.Key, color.Value);
				}
			}
			catch (Exception ex)
			{
				DebugWindow.LogError(ex.Message, 5f);
			}
		}
	}

	private unsafe ThemeConfig ReadThemeFromCurrent()
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		ThemeConfig themeConfig = new ThemeConfig
		{
			AntiAliasedLines = style.AntiAliasedLines,
			DisplaySafeAreaPadding = style.DisplaySafeAreaPadding,
			DisplayWindowPadding = style.DisplayWindowPadding,
			GrabRounding = style.GrabRounding,
			GrabMinSize = style.GrabMinSize,
			ScrollbarRounding = style.ScrollbarRounding,
			ScrollbarSize = style.ScrollbarSize,
			ColumnsMinSpacing = style.ColumnsMinSpacing,
			IndentSpacing = style.IndentSpacing,
			TouchExtraPadding = style.TouchExtraPadding,
			ItemInnerSpacing = style.ItemInnerSpacing,
			ItemSpacing = style.ItemSpacing,
			FrameRounding = style.FrameRounding,
			FramePadding = style.FramePadding,
			ChildWindowRounding = style.ChildRounding,
			WindowTitleAlign = style.WindowTitleAlign,
			WindowRounding = style.WindowRounding,
			WindowPadding = style.WindowPadding,
			Alpha = style.Alpha,
			AntiAliasedFill = style.AntiAliasedFill,
			CurveTessellationTolerance = style.CurveTessellationTol
		};
		ImGuiCol[] values = Enum.GetValues<ImGuiCol>();
		foreach (ImGuiCol imGuiCol in values)
		{
			if (imGuiCol != ImGuiCol.COUNT)
			{
				Vector4* styleColorVec = ImGui.GetStyleColorVec4(imGuiCol);
				themeConfig.Colors.Add(imGuiCol, new Vector4(styleColorVec->X, styleColorVec->Y, styleColorVec->Z, styleColorVec->W));
			}
		}
		return themeConfig;
	}

	private static ThemeConfig GenerateDefaultTheme()
	{
		return new ThemeConfig
		{
			Colors = 
			{
				{
					ImGuiCol.Text,
					new Vector4(0.9f, 0.9f, 0.9f, 1f)
				},
				{
					ImGuiCol.TextDisabled,
					new Vector4(0.6f, 0.6f, 0.6f, 1f)
				},
				{
					ImGuiCol.WindowBg,
					new Vector4(0.16f, 0.16f, 0.16f, 1f)
				},
				{
					ImGuiCol.ChildBg,
					new Vector4(0.12f, 0.12f, 0.12f, 1f)
				},
				{
					ImGuiCol.PopupBg,
					new Vector4(0.11f, 0.11f, 0.14f, 0.92f)
				},
				{
					ImGuiCol.Border,
					new Vector4(0.44f, 0.44f, 0.44f, 1f)
				},
				{
					ImGuiCol.BorderShadow,
					new Vector4(0f, 0f, 0f, 0f)
				},
				{
					ImGuiCol.FrameBg,
					new Vector4(0.2f, 0.2f, 0.2f, 1f)
				},
				{
					ImGuiCol.FrameBgHovered,
					new Vector4(0.98f, 0.61f, 0.26f, 1f)
				},
				{
					ImGuiCol.FrameBgActive,
					new Vector4(0.74f, 0.36f, 0.02f, 1f)
				},
				{
					ImGuiCol.TitleBg,
					new Vector4(0.4f, 0.19f, 0f, 1f)
				},
				{
					ImGuiCol.TitleBgActive,
					new Vector4(0.74f, 0.36f, 0f, 1f)
				},
				{
					ImGuiCol.TitleBgCollapsed,
					new Vector4(0.75f, 0.37f, 0f, 1f)
				},
				{
					ImGuiCol.MenuBarBg,
					new Vector4(0.29f, 0.29f, 0.3f, 1f)
				},
				{
					ImGuiCol.ScrollbarBg,
					new Vector4(0.28f, 0.28f, 0.28f, 1f)
				},
				{
					ImGuiCol.ScrollbarGrab,
					new Vector4(0.71f, 0.37f, 0f, 1f)
				},
				{
					ImGuiCol.ScrollbarGrabHovered,
					new Vector4(0.86f, 0.41f, 0.06f, 1f)
				},
				{
					ImGuiCol.ScrollbarGrabActive,
					new Vector4(0.64f, 0.29f, 0f, 1f)
				},
				{
					ImGuiCol.CheckMark,
					new Vector4(0.96f, 0.45f, 0.01f, 1f)
				},
				{
					ImGuiCol.SliderGrab,
					new Vector4(0.86f, 0.48f, 0f, 1f)
				},
				{
					ImGuiCol.SliderGrabActive,
					new Vector4(0.52f, 0.31f, 0f, 1f)
				},
				{
					ImGuiCol.Button,
					new Vector4(0.73f, 0.37f, 0f, 1f)
				},
				{
					ImGuiCol.ButtonHovered,
					new Vector4(0.97f, 0.57f, 0f, 1f)
				},
				{
					ImGuiCol.ButtonActive,
					new Vector4(0.62f, 0.29f, 0.01f, 1f)
				},
				{
					ImGuiCol.Header,
					new Vector4(0.59f, 0.28f, 0f, 1f)
				},
				{
					ImGuiCol.HeaderHovered,
					new Vector4(0.74f, 0.35f, 0.02f, 1f)
				},
				{
					ImGuiCol.HeaderActive,
					new Vector4(0.88f, 0.45f, 0f, 1f)
				},
				{
					ImGuiCol.Separator,
					new Vector4(0.5f, 0.5f, 0.5f, 1f)
				},
				{
					ImGuiCol.SeparatorHovered,
					new Vector4(0.6f, 0.6f, 0.7f, 1f)
				},
				{
					ImGuiCol.SeparatorActive,
					new Vector4(0.7f, 0.7f, 0.9f, 1f)
				},
				{
					ImGuiCol.ResizeGrip,
					new Vector4(1f, 1f, 1f, 0.16f)
				},
				{
					ImGuiCol.ResizeGripHovered,
					new Vector4(0.78f, 0.82f, 1f, 0.6f)
				},
				{
					ImGuiCol.ResizeGripActive,
					new Vector4(0.78f, 0.82f, 1f, 0.9f)
				},
				{
					ImGuiCol.PlotLines,
					new Vector4(1f, 1f, 1f, 1f)
				},
				{
					ImGuiCol.PlotLinesHovered,
					new Vector4(0.9f, 0.7f, 0f, 1f)
				},
				{
					ImGuiCol.PlotHistogram,
					new Vector4(0.9f, 0.7f, 0f, 1f)
				},
				{
					ImGuiCol.PlotHistogramHovered,
					new Vector4(1f, 0.6f, 0f, 1f)
				},
				{
					ImGuiCol.TextSelectedBg,
					new Vector4(1f, 0.03f, 0.03f, 0.35f)
				},
				{
					ImGuiCol.ModalWindowDimBg,
					new Vector4(0.2f, 0.2f, 0.2f, 0.35f)
				},
				{
					ImGuiCol.DragDropTarget,
					new Vector4(1f, 1f, 0f, 0.9f)
				}
			}
		};
	}

	private static ThemeConfig LoadTheme(string fileName, bool nullIfNotFound)
	{
		try
		{
			string path = Path.Combine("config/themes", fileName + ".hudtheme");
			if (File.Exists(path))
			{
				return JsonConvert.DeserializeObject<ThemeConfig>(File.ReadAllText(path), SettingsContainer.jsonSettings);
			}
		}
		catch (Exception ex)
		{
			DebugWindow.LogError($"Error while loading theme {fileName}: {ex.Message}, Generating default one", 3f);
		}
		if (nullIfNotFound)
		{
			return null;
		}
		return GenerateDefaultTheme();
	}

	private static void SaveTheme(ThemeConfig theme, string fileName)
	{
		try
		{
			string path = Path.Combine("config/themes", fileName + ".hudtheme");
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			using StreamWriter streamWriter = new StreamWriter(File.Create(path));
			string value = JsonConvert.SerializeObject(theme, Formatting.Indented, SettingsContainer.jsonSettings);
			streamWriter.Write(value);
		}
		catch (Exception ex)
		{
			DebugWindow.LogError("Error while loading theme: " + ex.Message, 3f);
		}
	}
}
