using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace ExileCore.Shared.Helpers;

public static class ConvertHelper
{
	public static string ToShorten(double value, string format = "0")
	{
		double num = Math.Abs(value);
		if (num >= 1000000.0)
		{
			return (value / 1000000.0).ToString("F2") + "M";
		}
		if (num >= 1000.0)
		{
			return (value / 1000.0).ToString("F1") + "K";
		}
		return value.ToString(format);
	}

	public static SharpDX.Color ToBGRAColor(this string value)
	{
		if (!uint.TryParse(value, NumberStyles.HexNumber, null, out var result))
		{
			return SharpDX.Color.Black;
		}
		return SharpDX.Color.FromBgra(result);
	}

	public static SharpDX.Color? ConfigColorValueExtractor(this string[] line, int index)
	{
		if (!IsNotNull(line, index))
		{
			return null;
		}
		return line[index].ToBGRAColor();
	}

	public static string ConfigValueExtractor(this string[] line, int index)
	{
		if (!IsNotNull(line, index))
		{
			return null;
		}
		return line[index];
	}

	private static bool IsNotNull(string[] line, int index)
	{
		if (line.Length > index)
		{
			return !string.IsNullOrEmpty(line[index]);
		}
		return false;
	}

	public static System.Numerics.Vector3 ColorNodeToVector3(this ColorNode color)
	{
		SharpDX.Vector3 vector = color.Value.ToVector3();
		return new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
	}

	public static System.Numerics.Vector2 TranslateToNum(this SharpDX.Vector2 vector, float dx = 0f, float dy = 0f)
	{
		return new System.Numerics.Vector2(vector.X + dx, vector.Y + dy);
	}

	public static System.Numerics.Vector4 TranslateToNum(this SharpDX.Vector4 vector, float dx = 0f, float dy = 0f, float dz = 0f, float dw = 0f)
	{
		return new System.Numerics.Vector4(vector.X + dx, vector.Y + dy, vector.Z + dz, vector.W + dw);
	}

	public static System.Numerics.Vector3 TranslateToNum(this System.Numerics.Vector3 vector, float dx = 0f, float dy = 0f, float dz = 0f)
	{
		return new System.Numerics.Vector3(vector.X + dx, vector.Y + dy, vector.Z + dz);
	}

	public static string ToHex(this SharpDX.Color value)
	{
		return ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B));
	}

	public static SharpDX.Color ColorFromHsv(double hue, double saturation, double value)
	{
		int num = Convert.ToInt32(Math.Floor(hue / 60.0)) % 6;
		double num2 = hue / 60.0 - Math.Floor(hue / 60.0);
		value *= 255.0;
		byte b = Convert.ToByte(value);
		byte b2 = Convert.ToByte(value * (1.0 - saturation));
		byte b3 = Convert.ToByte(value * (1.0 - num2 * saturation));
		byte b4 = Convert.ToByte(value * (1.0 - (1.0 - num2) * saturation));
		return num switch
		{
			0 => new ColorBGRA(b, b4, b2, byte.MaxValue), 
			1 => new ColorBGRA(b3, b, b2, byte.MaxValue), 
			2 => new ColorBGRA(b2, b, b4, byte.MaxValue), 
			3 => new ColorBGRA(b2, b3, b, byte.MaxValue), 
			4 => new ColorBGRA(b4, b2, b, byte.MaxValue), 
			_ => new ColorBGRA(b, b2, b3, byte.MaxValue), 
		};
	}
}
