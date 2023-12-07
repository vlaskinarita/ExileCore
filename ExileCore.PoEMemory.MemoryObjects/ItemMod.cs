using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.Shared;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ItemMod : RemoteMemoryObject
{
	private static readonly char[] Digits = "0123456789".ToCharArray();

	public static readonly int STRUCT_SIZE = 56;

	private string _rawName;

	private ModsDat.ModRecord _record;

	[Obsolete("Use Values instead")]
	public int Value1
	{
		get
		{
			if (Values.Count <= 0)
			{
				return 0;
			}
			return Values[0];
		}
	}

	[Obsolete("Use Values instead")]
	public int Value2
	{
		get
		{
			if (Values.Count <= 1)
			{
				return 0;
			}
			return Values[1];
		}
	}

	[Obsolete("Use Values instead")]
	public int Value3
	{
		get
		{
			if (Values.Count <= 2)
			{
				return 0;
			}
			return Values[2];
		}
	}

	[Obsolete("Use Values instead")]
	public int Value4
	{
		get
		{
			if (Values.Count <= 3)
			{
				return 0;
			}
			return Values[3];
		}
	}

	public List<int> Values
	{
		get
		{
			long num = base.M.Read<long>(base.Address);
			long num2 = base.M.Read<long>(base.Address + 8);
			long num3 = (num2 - num) / 8;
			if (num3 < 0 || num3 > 10)
			{
				return new List<int>();
			}
			return base.M.ReadStructsArray<int>(num, num2, 4);
		}
	}

	public IntRange[] ValuesMinMax
	{
		get
		{
			if (_record == null)
			{
				ReadRecord();
			}
			return _record?.StatRange;
		}
	}

	public string RawName
	{
		get
		{
			if (_record == null)
			{
				ReadRecord();
			}
			return _rawName ?? string.Empty;
		}
	}

	public string Name
	{
		get
		{
			string rawName = RawName;
			int num = rawName.IndexOfAny(Digits);
			return ((num == -1) ? rawName : rawName.Substring(0, num)).Replace("_", "");
		}
	}

	public string Group
	{
		get
		{
			if (_record == null)
			{
				ReadRecord();
			}
			return _record?.Group ?? string.Empty;
		}
	}

	public string DisplayName
	{
		get
		{
			if (_record == null)
			{
				ReadRecord();
			}
			return _record?.UserFriendlyName ?? string.Empty;
		}
	}

	public int Level
	{
		get
		{
			if (_record == null)
			{
				ReadRecord();
			}
			return _record?.MinLevel ?? 0;
		}
	}

	public ModsDat.ModRecord ModRecord
	{
		get
		{
			if (_record == null)
			{
				ReadRecord();
			}
			return _record;
		}
	}

	private void ReadRecord()
	{
		long address = base.M.Read<long>(base.Address + 40);
		_record = base.TheGame.Files.Mods.GetModByAddress(address);
		_rawName = _record?.Key;
	}

	public override string ToString()
	{
		IntRange[] valuesMinMax = ValuesMinMax;
		int count = Math.Min(Values.Count, valuesMinMax.Length);
		IEnumerable<string> values = Values.Take(count).Select(delegate(int x, int i)
		{
			IntRange intRange = ValuesMinMax[i];
			return (intRange.Min != intRange.Max) ? $"{x} [{intRange.Min}-{intRange.Max}]" : x.ToString();
		});
		return _rawName + " (" + string.Join(", ", values) + ")";
	}
}
