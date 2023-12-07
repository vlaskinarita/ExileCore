using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExileCore.PoEMemory;
using ExileCore.RenderQ;
using ExileCore.Shared.AtlasHelper;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ImGuiNET;
using SharpDX;

namespace ExileCore;

public class Graphics
{
	private record SetTextScaleDisposable(ImGuiRender Render, float OldScale) : IDisposable
	{
		public void Dispose()
		{
			Render.TextScale = OldScale;
		}
	}

	private static readonly RectangleF DefaultUV = new RectangleF(0f, 0f, 1f, 1f);

	private readonly CoreSettings _settings;

	private readonly ImGuiRender ImGuiRender;

	private readonly DX11 _lowLevel;

	[Obsolete]
	public DX11 LowLevel => _lowLevel;

	public FontContainer Font => ImGuiRender.CurrentFont;

	public FontContainer LastFont => ImGuiRender.CurrentFont;

	public Graphics(DX11 dx11, CoreSettings settings)
	{
		_lowLevel = dx11;
		_settings = settings;
		ImGuiRender = dx11.ImGuiRender;
	}

	public System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position, Color color, int height)
	{
		return DrawText(text, position, color, height, _settings.Font);
	}

	public System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position, Color color, FontAlign align)
	{
		return DrawText(text, position, color, _settings.FontSize, _settings.Font, align);
	}

	public System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position, Color color, string fontName, FontAlign align)
	{
		return ImGuiRender.DrawText(text, position, color, -1, fontName, align);
	}

	public System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position, Color color, int height, FontAlign align)
	{
		return ImGuiRender.DrawText(text, position, color, height, _settings.Font, align);
	}

	public System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position, Color color, int height, string fontName, FontAlign align = FontAlign.Left)
	{
		return ImGuiRender.DrawText(text, position, color, height, fontName, align);
	}

	public System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position, Color color)
	{
		return DrawText(text, position, color, _settings.FontSize, _settings.Font);
	}

	public System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position)
	{
		return DrawText(text, position, Color.White);
	}

	public System.Numerics.Vector2 DrawText(string text, System.Numerics.Vector2 position, FontAlign align)
	{
		return DrawText(text, position, Color.White, _settings.FontSize, align);
	}

	public IDisposable SetTextScale(float textScale)
	{
		float textScale2 = ImGuiRender.TextScale;
		ImGuiRender.TextScale *= textScale;
		return new SetTextScaleDisposable(ImGuiRender, textScale2);
	}

	public System.Numerics.Vector2 MeasureText(string text)
	{
		return ImGuiRender.MeasureText(text);
	}

	public System.Numerics.Vector2 MeasureText(string text, int height)
	{
		return ImGuiRender.MeasureText(text, height);
	}

	public void DrawLine(System.Numerics.Vector2 p1, System.Numerics.Vector2 p2, float borderWidth, Color color)
	{
		ImGuiRender.LowLevelApi.AddLine(p1, p2, color.ToImgui(), borderWidth);
	}

	public void DrawCircle(System.Numerics.Vector2 center, float radius, Color color)
	{
		DrawCircle(center, radius, color, 1f, 0);
	}

	public void DrawCircle(System.Numerics.Vector2 center, float radius, Color color, float thickness)
	{
		DrawCircle(center, radius, color, thickness, 0);
	}

	public void DrawCircle(System.Numerics.Vector2 center, float radius, Color color, int numSegments)
	{
		DrawCircle(center, radius, color, 1f, numSegments);
	}

	public void DrawCircle(System.Numerics.Vector2 center, float radius, Color color, float thickness, int numSegments)
	{
		ImGuiRender.LowLevelApi.AddCircle(center, radius, color.ToImgui(), numSegments, thickness);
	}

	public void DrawCircleFilled(System.Numerics.Vector2 center, float radius, Color color, int numSegments)
	{
		ImGuiRender.LowLevelApi.AddCircleFilled(center, radius, color.ToImgui(), numSegments);
	}

	public void DrawFilledCircleInWorld(System.Numerics.Vector3 worldCenter, float radius, Color color)
	{
		DrawFilledCircleInWorld(worldCenter, radius, color, 15);
	}

	public void DrawFilledCircleInWorld(System.Numerics.Vector3 worldCenter, float radius, Color color, int segmentCount)
	{
		System.Numerics.Vector2[] inWorldCirclePoints = GetInWorldCirclePoints(worldCenter, radius, segmentCount, addFinalPoint: false);
		DrawConvexPolyFilled(inWorldCirclePoints, color);
	}

	public void DrawCircleInWorld(System.Numerics.Vector3 worldCenter, float radius, Color color)
	{
		DrawCircleInWorld(worldCenter, radius, color, 1f);
	}

	public void DrawCircleInWorld(System.Numerics.Vector3 worldCenter, float radius, Color color, float thickness)
	{
		DrawCircleInWorld(worldCenter, radius, color, thickness, 15);
	}

	public void DrawCircleInWorld(System.Numerics.Vector3 worldCenter, float radius, Color color, float thickness, int segmentCount)
	{
		System.Numerics.Vector2[] inWorldCirclePoints = GetInWorldCirclePoints(worldCenter, radius, segmentCount, addFinalPoint: true);
		DrawPolyLine(inWorldCirclePoints, color, thickness);
	}

	private static System.Numerics.Vector2[] GetInWorldCirclePoints(System.Numerics.Vector3 worldCenter, float radius, int segmentCount, bool addFinalPoint)
	{
		System.Numerics.Vector2[] array = new System.Numerics.Vector2[segmentCount + (addFinalPoint ? 1 : 0)];
		float num = (float)Math.PI * 2f / (float)segmentCount;
		for (int i = 0; i < segmentCount + (addFinalPoint ? 1 : 0); i++)
		{
			float angle = (float)i * num;
			System.Numerics.Vector2 value = System.Numerics.Vector2.UnitX.RotateRadians(angle) * radius;
			System.Numerics.Vector3 vec = worldCenter + new System.Numerics.Vector3(value, 0f);
			array[i] = RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen(vec);
		}
		return array;
	}

	public void DrawPolyLine(System.Numerics.Vector2[] points, Color color)
	{
		DrawPolyLine(points, color, 1f, ImDrawFlags.None);
	}

	public void DrawPolyLine(System.Numerics.Vector2[] points, Color color, float thickness)
	{
		DrawPolyLine(points, color, thickness, ImDrawFlags.None);
	}

	public void DrawPolyLine(System.Numerics.Vector2[] points, Color color, float thickness, ImDrawFlags drawFlags)
	{
		ImGuiRender.LowLevelApi.AddPolyline(ref points[0], points.Length, color.ToImgui(), drawFlags, thickness);
	}

	public void DrawConvexPolyFilled(System.Numerics.Vector2[] points, Color color)
	{
		ImGuiRender.LowLevelApi.AddConvexPolyFilled(ref points[0], points.Length, color.ToImgui());
	}

	public void DrawFrame(System.Numerics.Vector2 p1, System.Numerics.Vector2 p2, Color color, float rounding, int thickness, int flags)
	{
		ImGuiRender.LowLevelApi.AddRect(p1, p2, color.ToImgui(), rounding, (ImDrawFlags)flags, thickness);
	}

	public void DrawBox(System.Numerics.Vector2 p1, System.Numerics.Vector2 p2, Color color, float rounding = 0f)
	{
		ImGuiRender.LowLevelApi.AddRectFilled(p1, p2, color.ToImgui(), rounding);
	}

	public void DrawQuad(System.Numerics.Vector2 a, System.Numerics.Vector2 b, System.Numerics.Vector2 c, System.Numerics.Vector2 d, Color color)
	{
		ImGuiRender.LowLevelApi.AddQuad(a, b, c, d, color.ToImgui());
	}

	public void DrawQuad(IntPtr textureId, System.Numerics.Vector2 a, System.Numerics.Vector2 b, System.Numerics.Vector2 c, System.Numerics.Vector2 d)
	{
		ImGuiRender.LowLevelApi.AddImageQuad(textureId, a, b, c, d);
	}

	public void DrawQuad(IntPtr textureId, System.Numerics.Vector2 a, System.Numerics.Vector2 b, System.Numerics.Vector2 c, System.Numerics.Vector2 d, Color color)
	{
		System.Numerics.Vector2 uv = default(System.Numerics.Vector2);
		System.Numerics.Vector2 uv2 = new System.Numerics.Vector2(1f, 0f);
		System.Numerics.Vector2 uv3 = new System.Numerics.Vector2(1f, 1f);
		System.Numerics.Vector2 uv4 = new System.Numerics.Vector2(0f, 1f);
		ImGuiRender.LowLevelApi.AddImageQuad(textureId, a, b, c, d, uv, uv2, uv3, uv4, color.ToImgui());
	}

	public void DrawImage(string fileName, RectangleF rectangle)
	{
		DrawImage(fileName, rectangle, DefaultUV, Color.White);
	}

	public void DrawImage(string fileName, RectangleF rectangle, Color color)
	{
		DrawImage(fileName, rectangle, DefaultUV, color);
	}

	public void DrawImage(string fileName, RectangleF rectangle, RectangleF uv, Color color)
	{
		try
		{
			ImGuiRender.DrawImage(fileName, rectangle, uv, color);
		}
		catch (Exception ex)
		{
			DebugWindow.LogError(ex.ToString());
		}
	}

	public void DrawImage(string fileName, RectangleF rectangle, RectangleF uv)
	{
		DrawImage(fileName, rectangle, uv, Color.White);
	}

	public void DrawImage(AtlasTexture atlasTexture, RectangleF rectangle)
	{
		DrawImage(atlasTexture, rectangle, Color.White);
	}

	public void DrawImage(AtlasTexture atlasTexture, RectangleF rectangle, Color color)
	{
		DrawImage(atlasTexture.AtlasFileName, rectangle, atlasTexture.TextureUV, color);
	}

	public void DrawImageGui(string fileName, RectangleF rectangle, RectangleF uv)
	{
		ImGuiRender.DrawImage(fileName, rectangle, uv);
	}

	public void DrawImageGui(string fileName, System.Numerics.Vector2 TopLeft, System.Numerics.Vector2 BottomRight, System.Numerics.Vector2 TopLeft_UV, System.Numerics.Vector2 BottomRight_UV)
	{
		ImGuiRender.DrawImage(fileName, TopLeft, BottomRight, TopLeft_UV, BottomRight_UV);
	}

	public void DrawBox(RectangleF rect, Color color)
	{
		DrawBox(rect, color, 0f);
	}

	public void DrawBox(RectangleF rect, Color color, float rounding)
	{
		DrawBox(rect.TopLeft.ToVector2Num(), rect.BottomRight.ToVector2Num(), color, rounding);
	}

	public void DrawFrame(RectangleF rect, Color color, float rounding, int thickness, int flags)
	{
		DrawFrame(rect.TopLeft.ToVector2Num(), rect.BottomRight.ToVector2Num(), color, rounding, thickness, flags);
	}

	public void DrawFrame(RectangleF rect, Color color, int thickness)
	{
		DrawFrame(rect.TopLeft.ToVector2Num(), rect.BottomRight.ToVector2Num(), color, 0f, thickness, 0);
	}

	public void DrawFrame(System.Numerics.Vector2 p1, System.Numerics.Vector2 p2, Color color, int thickness)
	{
		DrawFrame(p1, p2, color, 0f, thickness, 0);
	}

	public void DrawBoundingBoxInWorld(System.Numerics.Vector3 position, Color color, System.Numerics.Vector3 bounds, float rotationRadians)
	{
		DrawBoundingBoxInWorld(position, color, bounds, rotationRadians, bounds.Z * 2f);
	}

	public void DrawBoundingBoxInWorld(System.Numerics.Vector3 position, Color color, System.Numerics.Vector3 bounds, float rotationRadians, float height)
	{
		System.Numerics.Vector3 bnds2 = new System.Numerics.Vector3(bounds.Xy().RotateRadians(rotationRadians), bounds.Z);
		List<System.Numerics.Vector3> source = GetBoundingBoxPoints(position, bnds2, 0f);
		List<System.Numerics.Vector3> source2 = GetBoundingBoxPoints(position, bnds2, 0f - height);
		System.Numerics.Vector2[] array = source.Select(RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen).ToArray();
		System.Numerics.Vector2[] array2 = source2.Select(RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen).ToArray();
		DrawConvexPolyFilled(array, color);
		DrawConvexPolyFilled(array2, color);
		for (int i = 0; i < 4; i++)
		{
			DrawLine(array[i], array2[i], 2f, color);
		}
		static List<System.Numerics.Vector3> GetBoundingBoxPoints(System.Numerics.Vector3 pos, System.Numerics.Vector3 bnds, float offsetZ)
		{
			return new List<System.Numerics.Vector2>
			{
				bnds.Xy(),
				bnds.Xy().Rotate90DegreesCounterClockwise(),
				-bnds.Xy(),
				bnds.Xy().Rotate90DegreesClockwise()
			}.Select((System.Numerics.Vector2 b) => new System.Numerics.Vector3(pos.X + b.X, pos.Y + b.Y, pos.Z + offsetZ)).ToList();
		}
	}

	public void DrawBoundingCylinderInWorld(System.Numerics.Vector3 position, Color color, System.Numerics.Vector3 bounds, float rotationRadians)
	{
		DrawBoundingCylinderInWorld(position, color, bounds, rotationRadians, bounds.Z * 2f);
	}

	public void DrawBoundingCylinderInWorld(System.Numerics.Vector3 position, Color color, System.Numerics.Vector3 bounds, float rotationRadians, float height)
	{
		System.Numerics.Vector3 bounds2 = new System.Numerics.Vector3(bounds.Xy().RotateRadians(rotationRadians), bounds.Z);
		List<System.Numerics.Vector3> source = GetCylinderBasePoints(position, bounds2, 0f);
		List<System.Numerics.Vector3> source2 = GetCylinderBasePoints(position, bounds2, 0f - height);
		System.Numerics.Vector2[] array = source.Select(RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen).ToArray();
		System.Numerics.Vector2[] array2 = source2.Select(RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			DrawLine(array[i], array2[i], 2f, color);
		}
		DrawPolyLine(array, color, 2f);
		DrawPolyLine(array2, color, 2f);
		static List<System.Numerics.Vector3> GetCylinderBasePoints(System.Numerics.Vector3 pos, System.Numerics.Vector3 bounds, float offsetZ)
		{
			List<System.Numerics.Vector3> list = new List<System.Numerics.Vector3>(8);
			for (int j = 0; j < 8; j++)
			{
				System.Numerics.Vector2 value = pos.Xy() + bounds.Xy().RotateRadians((float)Math.PI * (float)j / 4f);
				list.Add(new System.Numerics.Vector3(value, pos.Z + offsetZ));
			}
			return list;
		}
	}

	public bool InitImage(string name, bool textures = true)
	{
		string name2 = (textures ? ("textures/" + name) : name);
		return LowLevel.InitTexture(name2);
	}

	public bool InitImage(string name, string path)
	{
		return LowLevel.InitTexture(name, path);
	}

	public IntPtr GetTextureId(string name)
	{
		return LowLevel.GetTexture(name);
	}

	public void DisposeTexture(string name)
	{
		LowLevel.DisposeTexture(name);
	}

	public IDisposable UseCurrentFont()
	{
		return ImGuiRender.UseCurrentFont();
	}

	[Obsolete]
	public System.Numerics.Vector2 DrawText(string text, SharpDX.Vector2 position, Color color, string fontName = null, FontAlign align = FontAlign.Left)
	{
		return ImGuiRender.DrawText(text, position.ToVector2Num(), color, -1, fontName, align);
	}

	[Obsolete]
	public System.Numerics.Vector2 DrawText(string text, SharpDX.Vector2 position, Color color)
	{
		return DrawText(text, position.ToVector2Num(), color, _settings.FontSize, _settings.Font);
	}

	[Obsolete]
	public System.Numerics.Vector2 DrawText(string text, SharpDX.Vector2 position, Color color, int height)
	{
		return DrawText(text, position.ToVector2Num(), color, height, _settings.Font);
	}

	[Obsolete]
	public System.Numerics.Vector2 DrawText(string text, SharpDX.Vector2 position, Color color, FontAlign align = FontAlign.Left)
	{
		return DrawText(text, position.ToVector2Num(), color, _settings.FontSize, _settings.Font, align);
	}

	[Obsolete]
	public System.Numerics.Vector2 DrawText(string text, SharpDX.Vector2 position, Color color, int height, FontAlign align = FontAlign.Left)
	{
		return DrawText(text, position.ToVector2Num(), color, height, _settings.Font, align);
	}

	[Obsolete]
	public System.Numerics.Vector2 DrawText(string text, SharpDX.Vector2 position, FontAlign align = FontAlign.Left)
	{
		return DrawText(text, position.ToVector2Num(), Color.White, _settings.FontSize, align);
	}

	[Obsolete]
	public void DrawLine(SharpDX.Vector2 p1, SharpDX.Vector2 p2, float borderWidth, Color color)
	{
		ImGuiRender.LowLevelApi.AddLine(p1.ToVector2Num(), p2.ToVector2Num(), color.ToImgui(), borderWidth);
	}

	[Obsolete]
	public void DrawBox(SharpDX.Vector2 p1, SharpDX.Vector2 p2, Color color, float rounding = 0f)
	{
		ImGuiRender.LowLevelApi.AddRectFilled(p1.ToVector2Num(), p2.ToVector2Num(), color.ToImgui(), rounding);
	}

	[Obsolete]
	public void DrawTexture(IntPtr user_texture_id, SharpDX.Vector2 a, SharpDX.Vector2 b, SharpDX.Vector2 c, SharpDX.Vector2 d)
	{
		ImGuiRender.LowLevelApi.AddImageQuad(user_texture_id, a.ToVector2Num(), b.ToVector2Num(), c.ToVector2Num(), d.ToVector2Num());
	}

	[Obsolete]
	public void DrawFrame(SharpDX.Vector2 p1, SharpDX.Vector2 p2, Color color, int thickness)
	{
		DrawFrame(p1.ToVector2Num(), p2.ToVector2Num(), color, 0f, thickness, 0);
	}
}
