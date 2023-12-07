using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory;

public class StringPattern : IPattern
{
	private string _mask;

	public string Name { get; }

	public byte[] Bytes { get; }

	public bool[] Mask { get; }

	public int StartOffset { get; init; }

	public int PatternOffset { get; }

	string IPattern.Mask => _mask ?? (_mask = new string(Mask.Select((bool x) => (!x) ? '?' : 'x').ToArray()));

	public StringPattern(string pattern, string name)
	{
		List<string> list = pattern.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		int num = list.FindIndex((string x) => x == "^");
		if (num == -1)
		{
			num = 0;
		}
		else
		{
			list.RemoveAt(num);
		}
		PatternOffset = num;
		Bytes = list.Select((string x) => (byte)((!(x == "??")) ? byte.Parse(x, NumberStyles.HexNumber) : 0)).ToArray();
		Mask = list.Select((string x) => x != "??").ToArray();
		Span<bool> span = Mask.AsSpan();
		Span<byte> span2 = Bytes.AsSpan();
		Name = name;
		while (span.Length > 0 && !span[0])
		{
			int patternOffset = PatternOffset;
			PatternOffset = patternOffset - 1;
			ref Span<bool> reference = ref span;
			span = reference.Slice(1, reference.Length - 1);
			ref Span<byte> reference2 = ref span2;
			span2 = reference2.Slice(1, reference2.Length - 1);
		}
		while (span.Length > 0)
		{
			if (span[span.Length - 1])
			{
				break;
			}
			ref Span<bool> reference = ref span;
			span = reference.Slice(0, reference.Length - 1);
			ref Span<byte> reference2 = ref span2;
			span2 = reference2.Slice(0, reference2.Length - 1);
		}
		Mask = span.ToArray();
		Bytes = span2.ToArray();
	}
}
