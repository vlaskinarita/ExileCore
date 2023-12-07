using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using ExileCore.Shared.Interfaces;
using GameOffsets;
using GameOffsets.Native;
using SharpDX;

namespace ExileCore.Shared.Helpers;

public static class MiscHelpers
{
	public static string InsertBeforeUpperCase(this string str, string append)
	{
		StringBuilder stringBuilder = new StringBuilder();
		char c = '\0';
		string text = str ?? "";
		foreach (char c2 in text)
		{
			if (char.IsUpper(c2) && stringBuilder.Length != 0 && c != ' ')
			{
				stringBuilder.Append(append);
			}
			stringBuilder.Append(c2);
			c = c2;
		}
		return stringBuilder.ToString();
	}

	public static string GetTimeString(TimeSpan timeSpent)
	{
		int num = (int)timeSpent.TotalSeconds;
		int num2 = num % 60;
		int num3 = num / 60;
		int num4 = num3 / 60;
		num3 %= 60;
		return string.Format((num4 > 0) ? "{0}:{1:00}:{2:00}" : "{1}:{2:00}", num4, num3, num2);
	}

	public static string ToString(this NativeStringU str, IMemory mem)
	{
		return ((NativeUtf16Text)str).ToString(mem);
	}

	public static string ToString(this NativeUtf16Text str, IMemory mem)
	{
		return str.ToString(mem, 256);
	}

	public static string ToString(this NativeUtf16Text str, IMemory mem, int maxLength)
	{
		if (str.Length <= 0)
		{
			return string.Empty;
		}
		if (str.LengthWithNullTerminator >= 8)
		{
			return mem.ReadStringU(str.Buffer, (int)(Math.Min(maxLength, str.Length) * 2));
		}
		if (str.Length > 8)
		{
			return "";
		}
		Span<byte> destination = stackalloc byte[16];
		BitConverter.TryWriteBytes(destination, str.Buffer);
		BitConverter.TryWriteBytes(destination.Slice(8, destination.Length - 8), str.Reserved8Bytes);
		return Encoding.Unicode.GetString(destination.Slice(0, (int)str.ByteLength));
	}

	public static string ToString(this NativeUtf8Text str, IMemory m)
	{
		if (str.Length <= 0)
		{
			return string.Empty;
		}
		if (str.LengthWithNullTerminator > 15)
		{
			return m.ReadString(str.Buffer, Math.Min(str.Length, 256));
		}
		Span<byte> destination = stackalloc byte[16];
		BitConverter.TryWriteBytes(destination, str.Buffer);
		BitConverter.TryWriteBytes(destination.Slice(8, destination.Length - 8), str.Reserved8Bytes);
		return Encoding.UTF8.GetString(destination.Slice(0, str.Length));
	}

	public static string ToString(this PathEntityOffsets str, IMemory mem)
	{
		return mem.ReadStringU(str.Path.Ptr, (int)str.Length * 2);
	}

	public static T ToEnum<T>(this string value)
	{
		return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
	}

	public static System.Numerics.Vector2 ClickRandomNum(this SharpDX.RectangleF clientRect, int x = 3, int y = 3)
	{
		int num = Random.Shared.Next((int)clientRect.TopLeft.X + x, (int)clientRect.TopRight.X - x);
		int num2 = Random.Shared.Next((int)clientRect.TopLeft.Y + y, (int)clientRect.BottomLeft.Y - x);
		return new System.Numerics.Vector2(num, num2);
	}

	public static System.Numerics.Vector2 ClickRandom(this System.Drawing.RectangleF clientRect, int x = 3, int y = 3)
	{
		int num = Random.Shared.Next((int)clientRect.Left + x, (int)clientRect.Right - x);
		int num2 = Random.Shared.Next((int)clientRect.Top + y, (int)clientRect.Bottom - x);
		return new System.Numerics.Vector2(num, num2);
	}

	[Obsolete]
	public static SharpDX.Vector2 ClickRandom(this SharpDX.RectangleF clientRect, int x = 3, int y = 3)
	{
		int num = Random.Shared.Next((int)clientRect.TopLeft.X + x, (int)clientRect.TopRight.X - x);
		int num2 = Random.Shared.Next((int)clientRect.TopLeft.Y + y, (int)clientRect.BottomLeft.Y - x);
		return new SharpDX.Vector2(num, num2);
	}

	public static void PerfTimerLogMsg(Action act, string msg, float time = 3f, bool log = false)
	{
		using (new PerformanceTimer(msg, 0, delegate(string s, TimeSpan span)
		{
			DebugWindow.LogMsg($"{s} -> {span.TotalMilliseconds} ms.", time, SharpDX.Color.Zero.GetRandomColor());
		}, log: false))
		{
			act?.Invoke();
		}
	}

	public static IEnumerable<string[]> LoadConfigBase(string path, int columnsCount = 2)
	{
		return from line in File.ReadAllLines(path)
			where !string.IsNullOrWhiteSpace(line) && line.IndexOf(';') >= 0 && !line.StartsWith("#")
			select (from parts in line.Split(new char[1] { ';' }, columnsCount)
				select parts.Trim()).ToArray();
	}
}
