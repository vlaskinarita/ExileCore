using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using ClickableTransparentOverlay;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using SharpDX;

namespace ExileCore.RenderQ;

public class ImGuiRender
{
	private record PopFont : IDisposable
	{
		public void Dispose()
		{
			ImGui.PopFont();
		}
	}

	private readonly ActionOverlay _overlay;

	private const string DefaultFontName = "Default:13";

	[Obsolete]
	public static readonly ImGuiWindowFlags InvisibleWindow = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings;

	private ImDrawListPtr _backGroundTextWindowPtr;

	private ImDrawListPtr _backGroundWindowPtr;

	private readonly DX11 _dx11;

	private FontContainer _lastFontContainer;

	[Obsolete]
	public DX11 Dx11 => _dx11;

	public CoreSettings CoreSettings { get; }

	[Obsolete]
	public ImGuiIOPtr io => ImGui.GetIO();

	public Dictionary<string, FontContainer> fonts { get; } = new Dictionary<string, FontContainer>();


	public ImDrawListPtr LowLevelApi => _backGroundWindowPtr;

	public float TextScale { get; set; } = 1f;


	public FontContainer CurrentFont
	{
		get
		{
			if (!fonts.TryGetValue(CoreSettings.Font.Value ?? "", out var value) && !fonts.TryGetValue("Default:13", out value))
			{
				if (!fonts.Any())
				{
					return _lastFontContainer;
				}
				return fonts.First().Value;
			}
			return value;
		}
	}

	public ImGuiRender(DX11 dx11, ActionOverlay overlay, CoreSettings coreSettings)
	{
		_overlay = overlay;
		_dx11 = dx11;
		CoreSettings = coreSettings;
		Initialize();
	}

	private void Initialize()
	{
		ListNode fontGlyphRange = CoreSettings.FontGlyphRange;
		fontGlyphRange.OnValueSelected = (Action<string>)Delegate.Combine(fontGlyphRange.OnValueSelected, (Action<string>)delegate
		{
			SetManualFont();
		});
		try
		{
			using (new PerformanceTimer("Load manual fonts"))
			{
				try
				{
					SetManualFont();
				}
				catch (Exception value)
				{
					Core.Logger?.Error($"Cant load fonts -> {value}");
				}
			}
		}
		catch (DllNotFoundException)
		{
			throw new DllNotFoundException("Need put in directory cimgui.dll");
		}
	}

	private unsafe void SetManualFont()
	{
		_overlay.ReplaceFont(delegate(ImFontConfig* config)
		{
			ImGuiIOPtr iO = ImGui.GetIO();
			fonts["Default:13"] = new FontContainer(iO.Fonts.AddFontDefault(config), "Default", 13);
			if (Directory.Exists("fonts") && Directory.GetFiles("fonts").Contains("fonts\\config.ini"))
			{
				string[] array = File.ReadAllLines("fonts\\config.ini");
				IntPtr glyphRange = GetGlyphRange(CoreSettings.FontGlyphRange.Value);
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string[] array3 = array2[i].Split(':');
					string text = "fonts\\" + array3[0] + ".ttf";
					int num = int.Parse(array3[1]);
					if (!File.Exists(text))
					{
						DebugWindow.LogError($"Font {text} specified in {"fonts\\config.ini"} does not exist");
					}
					else
					{
						fonts[$"{text.Replace(".ttf", "").Replace("fonts\\", "")}:{num}"] = new FontContainer(iO.Fonts.AddFontFromFileTTF(text, num, config, glyphRange), text, num);
					}
				}
			}
			_lastFontContainer = fonts.First().Value;
			CoreSettings.Font.Values = fonts.Keys.ToList();
			if (CoreSettings.Font.Value == null || !fonts.ContainsKey(CoreSettings.Font.Value))
			{
				CoreSettings.Font.Value = CoreSettings.Font.Values.First();
			}
		});
	}

	private static IntPtr GetGlyphRange(string value)
	{
		ImGuiIOPtr iO = ImGui.GetIO();
		FontGlyphRangeType result;
		return (Enum.TryParse<FontGlyphRangeType>(value, out result) ? result : FontGlyphRangeType.Cyrillic) switch
		{
			FontGlyphRangeType.English => iO.Fonts.GetGlyphRangesDefault(), 
			FontGlyphRangeType.ChineseSimplifiedCommon => iO.Fonts.GetGlyphRangesChineseSimplifiedCommon(), 
			FontGlyphRangeType.ChineseFull => iO.Fonts.GetGlyphRangesChineseFull(), 
			FontGlyphRangeType.Japanese => iO.Fonts.GetGlyphRangesJapanese(), 
			FontGlyphRangeType.Korean => iO.Fonts.GetGlyphRangesKorean(), 
			FontGlyphRangeType.Thai => iO.Fonts.GetGlyphRangesThai(), 
			FontGlyphRangeType.Vietnamese => iO.Fonts.GetGlyphRangesVietnamese(), 
			FontGlyphRangeType.Cyrillic => iO.Fonts.GetGlyphRangesCyrillic(), 
			_ => iO.Fonts.GetGlyphRangesCyrillic(), 
		};
	}

	public void BeginBackGroundWindow()
	{
		ImGuiIOPtr iO = ImGui.GetIO();
		ImGui.SetNextWindowContentSize(iO.DisplaySize);
		ImGui.SetNextWindowPos(new System.Numerics.Vector2(0f, 0f));
		ImGui.Begin("Background Window", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus);
		_backGroundWindowPtr = ImGui.GetWindowDrawList();
		ImGui.End();
		ImGui.SetNextWindowContentSize(iO.DisplaySize);
		ImGui.SetNextWindowPos(new System.Numerics.Vector2(0f, 0f));
		ImGui.Begin("Background Text Window", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing);
		_backGroundTextWindowPtr = ImGui.GetWindowDrawList();
		ImGui.End();
	}

	public System.Numerics.Vector2 MeasureText(string text)
	{
		return ImGui.CalcTextSize(text) * TextScale;
	}

	public System.Numerics.Vector2 MeasureText(string text, int height)
	{
		return ImGui.CalcTextSize(text) * TextScale;
	}

	public unsafe System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position, Color color, int height, string fontName, FontAlign align)
	{
		try
		{
			bool num = fontName != "";
			FontContainer value;
			if (fontName == null)
			{
				value = (_lastFontContainer = CurrentFont);
			}
			else
			{
				if (!fonts.TryGetValue(fontName, out value))
				{
					value = fonts.First().Value;
					DebugWindow.LogError($"Font: {fontName} not found. Using: {value.Name}:{value.Size}");
				}
				_lastFontContainer = value;
			}
			using (num ? UseFont(value) : null)
			{
				System.Numerics.Vector2 result = MeasureText(text);
				if ((align & FontAlign.Center) != 0)
				{
					position.X -= result.X / 2f;
				}
				if ((align & FontAlign.VerticalCenter) != 0)
				{
					position.Y -= result.Y / 2f;
				}
				if ((align & FontAlign.Top) != 0)
				{
					position.Y -= result.Y;
				}
				if ((align & FontAlign.Right) != 0)
				{
					position.X -= result.X;
				}
				_backGroundTextWindowPtr.AddText(value.Atlas, (float)value.Size * TextScale, position, color.ToImgui(), text);
				return result;
			}
		}
		catch (Exception value2)
		{
			Console.WriteLine(value2);
			throw;
		}
	}

	public void DrawImage(string fileName, RectangleF rectangle, RectangleF uv)
	{
		DrawImage(fileName, rectangle, uv, Color.White);
	}

	public void DrawImage(string fileName, RectangleF rectangle, RectangleF uv, Color color)
	{
		_backGroundTextWindowPtr.AddImage(_dx11.GetTexture(fileName), rectangle.TopLeft.ToVector2Num(), rectangle.BottomRight.ToVector2Num(), uv.TopLeft.ToVector2Num(), uv.BottomRight.ToVector2Num(), color.ToImgui());
	}

	public void DrawImage(string filename, System.Numerics.Vector2 TopLeft, System.Numerics.Vector2 BottomRight, System.Numerics.Vector2 TopLeft_UV, System.Numerics.Vector2 BottomRight_UV)
	{
		_backGroundTextWindowPtr.AddImage(_dx11.GetTexture(filename), TopLeft, BottomRight, TopLeft_UV, BottomRight_UV);
	}

	[Description("Count Colors means how many colors used in text, if you use a lot colors need put number more than colors you have.This used for optimization.")]
	public System.Numerics.Vector2 DrawMultiColoredText(string text, System.Numerics.Vector2 position, FontAlign align = FontAlign.Left, int countColors = 10)
	{
		ReadOnlySpan<char> span = text.AsSpan();
		int num = 0;
		Span<uint> span2 = stackalloc uint[countColors];
		Span<int> span3 = stackalloc int[countColors * 2 + 1];
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] != '#' || i + 10 >= text.Length || text[i + 9] != '#')
			{
				continue;
			}
			span2[num3++] = span.Slice(i + 1, 8).HexToUInt();
			if (i != 0 && num == 0)
			{
				span3[num2++] = num;
				span3[num2++] = i;
				num = i + 10;
				span3[num2++] = num;
				i += 9;
				continue;
			}
			if (i != 0)
			{
				span3[num2++] = i - num;
			}
			num = i + 10;
			i += 9;
			span3[num2++] = num;
		}
		span3[num2++] = text.Length - num;
		int num4 = (int)Math.Ceiling((float)num2 / (float)num3);
		using (UseCurrentFont())
		{
			if (align == FontAlign.Center)
			{
				position.X -= MeasureText(text, CurrentFont.Size).X / 4f;
			}
			float x = position.X;
			if (num4 == 2)
			{
				int num5 = 0;
				for (int j = 0; j < num2; j += 2)
				{
					int start = span3[j];
					int len = span3[j + 1];
					uint clr = span2[num5++];
					DrawClrText2(ref span, ref position, x, align, start, len, clr);
				}
			}
			else
			{
				if (num4 < 3)
				{
					throw new Exception("Something wrong");
				}
				int num6 = 0;
				for (int k = 0; k < num2; k += 2)
				{
					int start2 = span3[k];
					int len2 = span3[k + 1];
					if (k == 0)
					{
						DrawClrText2(ref span, ref position, x, align, start2, len2, Color.White.ToImgui());
						continue;
					}
					uint clr2 = span2[num6++];
					DrawClrText2(ref span, ref position, x, align, start2, len2, clr2);
				}
			}
			return position;
		}
	}

	private unsafe System.Numerics.Vector2 DrawClrText2(ref ReadOnlySpan<char> span, ref System.Numerics.Vector2 position, float xStart, FontAlign align, int start, int len, uint clr)
	{
		string text = span.Slice(start, len).ToString();
		FontContainer currentFont = CurrentFont;
		System.Numerics.Vector2 result = MeasureText(text, currentFont.Size);
		switch (align)
		{
		case FontAlign.Left:
			_backGroundWindowPtr.AddText(currentFont.Atlas, currentFont.Size, position, clr, text);
			position.X += result.X;
			break;
		case FontAlign.Center:
			_backGroundWindowPtr.AddText(currentFont.Atlas, currentFont.Size, position, clr, text);
			position.X += result.X;
			break;
		case FontAlign.Right:
			position.X -= result.X;
			_backGroundWindowPtr.AddText(currentFont.Atlas, currentFont.Size, position, clr, text);
			break;
		}
		if (text[len - 1] == '\n')
		{
			position.X = xStart;
			position.Y += result.Y;
		}
		return result;
	}

	[Description("Count Colors means how many colors used in text, if you use a lot colors need put number more than colors you have.This used for optimization.")]
	public void MultiColoredText(string text, int countColors = 10)
	{
		ReadOnlySpan<char> span = text.AsSpan();
		int num = 0;
		Span<uint> span2 = stackalloc uint[countColors];
		Span<int> span3 = stackalloc int[countColors * 2 + 1];
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] != '#' || i + 10 >= text.Length || text[i + 9] != '#')
			{
				continue;
			}
			span2[num3++] = span.Slice(i + 1, 8).HexToUInt();
			if (i != 0 && num == 0)
			{
				span3[num2++] = num;
				span3[num2++] = i;
				num = i + 10;
				span3[num2++] = num;
				i += 9;
				continue;
			}
			if (i != 0)
			{
				span3[num2++] = i - num;
			}
			num = i + 10;
			i += 9;
			span3[num2++] = num;
		}
		span3[num2++] = text.Length - num;
		switch ((int)Math.Ceiling((float)num2 / (float)num3))
		{
		case 2:
		{
			int num5 = 0;
			for (int k = 0; k < num2; k += 2)
			{
				int start2 = span3[k];
				int len2 = span3[k + 1];
				uint color2 = span2[num5++];
				DrawClrText(ref span, start2, len2, Color.FromRgba(color2), k, num2);
			}
			break;
		}
		case 3:
		{
			int num4 = 0;
			for (int j = 0; j < num2; j += 2)
			{
				int start = span3[j];
				int len = span3[j + 1];
				if (j == 0)
				{
					DrawClrText(ref span, start, len, Color.Transparent, j, num2, noColor: true);
					continue;
				}
				uint color = span2[num4++];
				DrawClrText(ref span, start, len, Color.FromRgba(color), j, num2);
			}
			break;
		}
		default:
			throw new Exception("Something wrong");
		}
	}

	private unsafe void DrawClrText(ref ReadOnlySpan<char> span, int start, int len, Color clr, int index, int spanIndex, bool noColor = false)
	{
		ReadOnlySpan<char> readOnlySpan = span.Slice(start, len);
		fixed (char* ptr2 = readOnlySpan)
		{
			byte* ptr = stackalloc byte[(int)(uint)readOnlySpan.Length];
			Encoding.UTF8.GetBytes(ptr2, readOnlySpan.Length, ptr, readOnlySpan.Length);
			if (noColor)
			{
				ImGuiNative.igText(ptr);
			}
			else
			{
				ImGuiNative.igTextColored(clr.ToImguiVec4(), ptr);
			}
			if (index + 2 < spanIndex)
			{
				if (ptr2[len - 1] == '\n')
				{
					return;
				}
				ImGuiNative.igSameLine(0f, 0f);
			}
		}
	}

	public IDisposable UseCurrentFont()
	{
		return UseFont(CurrentFont);
	}

	private unsafe IDisposable UseFont(FontContainer container)
	{
		_lastFontContainer = container;
		ImGui.PushFont(container.Atlas);
		return new PopFont();
	}
}
