using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory;

public class Pattern : IPattern
{
	public string Name { get; }

	public byte[] Bytes { get; }

	public string Mask { get; }

	public int StartOffset { get; }

	public Pattern(byte[] pattern, string mask, string name, int startOffset = 0)
	{
		Bytes = pattern;
		Mask = Regex.Replace(mask, "\\s+", "");
		Name = name;
		StartOffset = startOffset;
	}

	public Pattern(string pattern, string mask, string name, int startOffset = 0)
	{
		string[] source = pattern.Split(new string[1] { "\\x" }, StringSplitOptions.RemoveEmptyEntries);
		Bytes = source.Select((string y) => byte.Parse(y, NumberStyles.HexNumber)).ToArray();
		Mask = mask;
		Name = name;
		StartOffset = startOffset;
	}
}
