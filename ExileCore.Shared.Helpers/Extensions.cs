using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets.Native;
using SharpDX;

namespace ExileCore.Shared.Helpers;

public static class Extensions
{
	private static readonly Color[] Colors;

	private static readonly Dictionary<string, MapIconsIndex> Icons;

	private static Dictionary<string, Color> ColorName { get; }

	private static Dictionary<Color, string> ColorHex { get; }

	static Extensions()
	{
		ColorName = new Dictionary<string, Color>();
		ColorHex = new Dictionary<Color, string>();
		FieldInfo[] fields = typeof(Color).GetFields(BindingFlags.Static | BindingFlags.Public);
		Colors = new Color[fields.Length];
		ColorName = new Dictionary<string, Color>(fields.Length);
		ColorHex = new Dictionary<Color, string>(fields.Length);
		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			Color color = (Color)fieldInfo.GetValue(typeof(Color));
			ColorName[fieldInfo.Name] = color;
			ColorName[fieldInfo.Name.ToLower()] = color;
			ColorHex[color] = color.ToRgba().ToString("X");
			if (color != Color.Transparent)
			{
				Colors[i] = color;
			}
		}
		Icons = new Dictionary<string, MapIconsIndex>(200);
		MapIconsIndex[] values = Enum.GetValues<MapIconsIndex>();
		for (int j = 0; j < values.Length; j++)
		{
			MapIconsIndex value = values[j];
			Icons[value.ToString()] = value;
		}
	}

	public static Color GetRandomColor(this Color c)
	{
		return Colors[Random.Shared.Next(0, Colors.Length - 1)];
	}

	public static MapIconsIndex IconIndexByName(string name)
	{
		Icons.TryGetValue(name, out var value);
		return value;
	}

	public static Color GetColorByName(string name)
	{
		if (!ColorName.TryGetValue(name, out var value))
		{
			return Color.Zero;
		}
		return value;
	}

	public static string Hex(this Color clr)
	{
		if (!ColorHex.TryGetValue(clr, out var value))
		{
			return ColorHex[Color.Transparent];
		}
		return value;
	}

	public static uint ToImgui(this Color c)
	{
		return (uint)c.ToRgba();
	}

	public static System.Numerics.Vector4 ToImguiVec4(this Color c)
	{
		return new System.Numerics.Vector4((float)(int)c.R / 255f, (float)(int)c.G / 255f, (float)(int)c.B / 255f, (float)(int)c.A / 255f);
	}

	public static System.Numerics.Vector4 ToImguiVec4(this Color c, byte alpha)
	{
		return new System.Numerics.Vector4((int)c.R, (int)c.G, (int)c.B, (int)alpha);
	}

	public static System.Numerics.Vector4 ToVector4Num(this SharpDX.Vector4 v)
	{
		return new System.Numerics.Vector4(v.X, v.Y, v.Z, v.W);
	}

	public static System.Numerics.Vector3 ToVector3Num(this SharpDX.Vector3 v)
	{
		return new System.Numerics.Vector3(v.X, v.Y, v.Z);
	}

	public static System.Numerics.Vector2 ToVector2Num(this SharpDX.Vector2 v)
	{
		return new System.Numerics.Vector2(v.X, v.Y);
	}

	public static System.Numerics.Vector2 ToVector2(this Point point)
	{
		return new System.Numerics.Vector2(point.X, point.Y);
	}

	public static SharpDX.Vector2 ToSharpDx(this System.Numerics.Vector2 v)
	{
		return new SharpDX.Vector2(v.X, v.Y);
	}

	public static SharpDX.Vector3 ToSharpDx(this System.Numerics.Vector3 v)
	{
		return new SharpDX.Vector3(v.X, v.Y, v.Z);
	}

	public static SharpDX.Vector4 ToSharpDx(this System.Numerics.Vector4 v)
	{
		return new SharpDX.Vector4(v.X, v.Y, v.Z, v.W);
	}

	public static System.Numerics.Vector2 Xy(this System.Numerics.Vector3 v)
	{
		return new System.Numerics.Vector2(v.X, v.Y);
	}

	public static System.Numerics.Vector2 Xy(this System.Numerics.Vector4 v)
	{
		return new System.Numerics.Vector2(v.X, v.Y);
	}

	public static System.Numerics.Vector3 Xyz(this System.Numerics.Vector4 v)
	{
		return new System.Numerics.Vector3(v.X, v.Y, v.Z);
	}

	public static Vector2i RoundToVector2I(this System.Numerics.Vector2 v)
	{
		return new Vector2i((int)MathF.Round(v.X), (int)MathF.Round(v.Y));
	}

	public static bool Contains(this RectangleF rectangle, System.Numerics.Vector2 v)
	{
		return rectangle.Contains(v.ToSharpDx());
	}

	public static Color ToSharpColor(this System.Numerics.Vector4 v)
	{
		return new Color(v.X, v.Y, v.Z, v.W);
	}

	public static uint ToImguiBase255(this System.Numerics.Vector4 v)
	{
		return (uint)((int)v.X | ((int)v.Y << 8) | ((int)v.Z << 16) | ((int)v.W << 24));
	}

	public static uint ToImguiBase1(this System.Numerics.Vector4 v)
	{
		return (v * 255f).ToImguiBase255();
	}

	public static int GetOffset<T>(Expression<Func<T, object>> selectExpression) where T : struct
	{
		try
		{
			return ((MemberExpression)((UnaryExpression)selectExpression.Body).Operand).Member.GetCustomAttribute<FieldOffsetAttribute>().Value;
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
			throw;
		}
	}

	public static ValidCache<T> ValidCache<T>(this Entity entity, Func<T> func)
	{
		return new ValidCache<T>(entity, func);
	}

	public static uint HexToUInt(this ReadOnlySpan<char> span)
	{
		uint num = 0u;
		for (int i = 0; i < span.Length; i++)
		{
			char c = span[i];
			if (num > 268435455)
			{
				return num;
			}
			num *= 16;
			if (c != 0)
			{
				uint num2 = num;
				num2 = ((c >= '0' && c <= '9') ? (num2 + (uint)(c - 48)) : ((c < 'A' || c > 'F') ? (num2 + (uint)(c - 97 + 10)) : (num2 + (uint)(c - 65 + 10))));
				if (num2 < num)
				{
					return num;
				}
				num = num2;
			}
		}
		return num;
	}
}
