using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using SharpDX;

namespace ExileCore.Shared.Helpers;

public static class MathHepler
{
	private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

	[Obsolete("Use Random.Shared")]
	public static Random Randomizer => Random.Shared;

	public static System.Numerics.Vector2 Rotate(this System.Numerics.Vector2 v, float angleDegrees)
	{
		return v.RotateRadians(ConvertToRadians(angleDegrees));
	}

	public static System.Numerics.Vector2 RotateRadians(this System.Numerics.Vector2 v, float angle)
	{
		(float Sin, float Cos) tuple = MathF.SinCos(angle);
		float item = tuple.Sin;
		float item2 = tuple.Cos;
		float x = v.X * item2 - v.Y * item;
		float y = v.X * item + v.Y * item2;
		return new System.Numerics.Vector2(x, y);
	}

	public static System.Numerics.Vector2 Rotate90DegreesCounterClockwise(this System.Numerics.Vector2 v)
	{
		return new System.Numerics.Vector2(0f - v.Y, v.X);
	}

	public static System.Numerics.Vector2 Rotate90DegreesClockwise(this System.Numerics.Vector2 v)
	{
		return new System.Numerics.Vector2(v.Y, 0f - v.X);
	}

	public static double ConvertToRadians(double angle)
	{
		return Math.PI / 180.0 * angle;
	}

	public static float ConvertToRadians(float angle)
	{
		return (float)Math.PI / 180f * angle;
	}

	public static double GetPolarCoordinates(this System.Numerics.Vector2 vector, out double phi)
	{
		double num = vector.Length();
		phi = Math.Acos((double)vector.X / num);
		if (vector.Y < 0f)
		{
			phi = Math.PI * 2.0 - phi;
		}
		return num;
	}

	public static string GetRandomWord(int length)
	{
		char[] array = new char[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"[Random.Shared.Next("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Length)];
		}
		return new string(array);
	}

	public static float Max(params float[] values)
	{
		float num = values.First();
		for (int i = 1; i < values.Length; i++)
		{
			num = Math.Max(num, values[i]);
		}
		return num;
	}

	public static System.Numerics.Vector2 Translate(this System.Numerics.Vector2 vector, float dx = 0f, float dy = 0f)
	{
		return new System.Numerics.Vector2(vector.X + dx, vector.Y + dy);
	}

	public static System.Numerics.Vector2 Translate(this System.Numerics.Vector2 vector, System.Numerics.Vector2 vector2)
	{
		return new System.Numerics.Vector2(vector.X + vector2.X, vector.Y + vector2.Y);
	}

	public static System.Numerics.Vector2 Mult(this System.Numerics.Vector2 vector, float dx = 1f, float dy = 1f)
	{
		return new System.Numerics.Vector2(vector.X * dx, vector.Y * dy);
	}

	public static System.Numerics.Vector3 Translate(this System.Numerics.Vector3 vector, float dx, float dy, float dz)
	{
		return new System.Numerics.Vector3(vector.X + dx, vector.Y + dy, vector.Z + dz);
	}

	public static float Distance(this System.Numerics.Vector2 a, System.Numerics.Vector2 b)
	{
		return System.Numerics.Vector2.Distance(a, b);
	}

	public static float DistanceSquared(this System.Numerics.Vector2 a, System.Numerics.Vector2 b)
	{
		return System.Numerics.Vector2.DistanceSquared(a, b);
	}

	public static bool IsInRectangle(this System.Numerics.Vector2 point, System.Drawing.RectangleF rect)
	{
		if (point.X >= rect.X && point.Y >= rect.Y && point.X <= rect.Width)
		{
			return point.Y <= rect.Height;
		}
		return false;
	}

	public static SharpDX.RectangleF GetDirectionsUV(double phi, double distance)
	{
		phi += Math.PI / 4.0;
		if (phi > Math.PI * 2.0)
		{
			phi -= Math.PI * 2.0;
		}
		float num = (float)Math.Round(phi / Math.PI * 4.0);
		if (num >= 8f)
		{
			num = 0f;
		}
		float num2 = ((distance > 60.0) ? ((!(distance > 120.0)) ? 1 : 2) : 0);
		float num3 = num / 8f;
		float num4 = num2 / 3f;
		return new SharpDX.RectangleF(num3, num4, (num + 1f) / 8f - num3, (num2 + 1f) / 3f - num4);
	}

	[Obsolete]
	public static SharpDX.Vector2 RotateVector2(SharpDX.Vector2 v, float angle)
	{
		float num = ConvertToRadians(angle);
		double num2 = Math.Cos(num);
		double num3 = Math.Sin(num);
		double num4 = (double)v.X * num2 - (double)v.Y * num3;
		double num5 = (double)v.X * num3 + (double)v.Y * num2;
		return new SharpDX.Vector2((float)num4, (float)num5);
	}

	[Obsolete]
	public static SharpDX.Vector2 NormalizeVector(SharpDX.Vector2 vec)
	{
		float num = VectorLength(vec);
		vec.X /= num;
		vec.Y /= num;
		return vec;
	}

	[Obsolete]
	public static float VectorLength(SharpDX.Vector2 vec)
	{
		return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
	}

	[Obsolete]
	public static double GetPolarCoordinates(this SharpDX.Vector2 vector, out double phi)
	{
		double num = vector.Length();
		phi = Math.Acos((double)vector.X / num);
		if (vector.Y < 0f)
		{
			phi = 6.2831854820251465 - phi;
		}
		return num;
	}

	[Obsolete]
	public static SharpDX.Vector2 Translate(this SharpDX.Vector2 vector, float dx = 0f, float dy = 0f)
	{
		return new SharpDX.Vector2(vector.X + dx, vector.Y + dy);
	}

	[Obsolete]
	public static SharpDX.Vector2 TranslateToNum(this System.Numerics.Vector2 vector, float dx = 0f, float dy = 0f)
	{
		return new SharpDX.Vector2(vector.X + dx, vector.Y + dy);
	}

	[Obsolete]
	public static SharpDX.Vector3 Translate(this SharpDX.Vector3 vector, float dx, float dy, float dz)
	{
		return new SharpDX.Vector3(vector.X + dx, vector.Y + dy, vector.Z + dz);
	}

	[Obsolete]
	public static float Distance(this SharpDX.Vector2 a, SharpDX.Vector2 b)
	{
		return SharpDX.Vector2.Distance(a, b);
	}

	[Obsolete]
	public static float DistanceSquared(this SharpDX.Vector2 a, SharpDX.Vector2 b)
	{
		return SharpDX.Vector2.DistanceSquared(a, b);
	}

	[Obsolete]
	public static bool PointInRectangle(this SharpDX.Vector2 point, SharpDX.RectangleF rect)
	{
		if (point.X >= rect.X && point.Y >= rect.Y && point.X <= rect.Width)
		{
			return point.Y <= rect.Height;
		}
		return false;
	}

	[Obsolete]
	public static bool PointInRectangle(this System.Numerics.Vector2 point, SharpDX.RectangleF rect)
	{
		if (point.X >= rect.X && point.Y >= rect.Y && point.X <= rect.Width)
		{
			return point.Y <= rect.Height;
		}
		return false;
	}
}
