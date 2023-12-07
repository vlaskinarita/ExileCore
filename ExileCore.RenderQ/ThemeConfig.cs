using System.Collections.Generic;
using System.Numerics;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;

namespace ExileCore.RenderQ;

public class ThemeConfig : ISettings
{
	public Dictionary<ImGuiCol, Vector4> Colors = new Dictionary<ImGuiCol, Vector4>();

	public ToggleNode Enable { get; set; }

	public bool AntiAliasedLines { get; set; } = true;


	public Vector2 DisplaySafeAreaPadding { get; set; } = Vector2.One * 8f;


	public Vector2 DisplayWindowPadding { get; set; } = Vector2.One * 8f;


	public float GrabRounding { get; set; }

	public float GrabMinSize { get; set; } = 10f;


	public float ScrollbarRounding { get; set; } = 9f;


	public float ScrollbarSize { get; set; } = 16f;


	public float ColumnsMinSpacing { get; set; } = 21f;


	public float IndentSpacing { get; set; } = 21f;


	public Vector2 TouchExtraPadding { get; set; } = Vector2.Zero;


	public Vector2 ItemInnerSpacing { get; set; } = Vector2.One * 4f;


	public Vector2 ItemSpacing { get; set; } = new Vector2(8f, 4f);


	public float FrameRounding { get; set; }

	public Vector2 FramePadding { get; set; } = new Vector2(4f, 3f);


	public float ChildWindowRounding { get; set; }

	public Vector2 WindowTitleAlign { get; set; } = Vector2.One * 0.5f;


	public float WindowRounding { get; set; } = 7f;


	public Vector2 WindowPadding { get; set; } = Vector2.One * 8f;


	public float Alpha { get; set; } = 1f;


	public bool AntiAliasedFill { get; set; } = true;


	public float CurveTessellationTolerance { get; set; } = 1f;


	public ThemeConfig()
	{
		Enable = new ToggleNode(value: true);
	}
}
