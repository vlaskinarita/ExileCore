using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class BaseItemTypes : FileInMemory
{
	public Dictionary<string, BaseItemType> Contents { get; } = new Dictionary<string, BaseItemType>();


	public Dictionary<long, BaseItemType> ContentsAddr { get; } = new Dictionary<long, BaseItemType>();


	public BaseItemTypes(IMemory m, Func<long> address)
		: base(m, address)
	{
		LoadItemTypes();
	}

	public BaseItemType GetFromAddress(long address)
	{
		ContentsAddr.TryGetValue(address, out var value);
		return value;
	}

	public BaseItemType Translate(string metadata)
	{
		if (Contents.Count == 0)
		{
			LoadItemTypes();
		}
		if (metadata == null)
		{
			return null;
		}
		if (!Contents.TryGetValue(metadata, out var value))
		{
			Console.WriteLine("Key not found in BaseItemTypes: " + metadata);
			return null;
		}
		return value;
	}

	private void LoadItemTypes()
	{
		foreach (long item in RecordAddresses())
		{
			string text = base.M.ReadStringU(base.M.Read<long>(item));
			BaseItemType baseItemType = new BaseItemType();
			baseItemType.Metadata = text;
			baseItemType.ClassName = base.M.ReadStringU(base.M.Read<long>(item + 8, new int[1]));
			baseItemType.Width = base.M.Read<int>(item + 24);
			baseItemType.Height = base.M.Read<int>(item + 28);
			baseItemType.BaseName = base.M.ReadStringU(base.M.Read<long>(item + 32));
			baseItemType.DropLevel = base.M.Read<int>(item + 48);
			baseItemType.Tags = new string[base.M.Read<long>(item + 104)];
			BaseItemType baseItemType2 = baseItemType;
			long num = base.M.Read<long>(item + 112);
			for (int i = 0; i < baseItemType2.Tags.Length; i++)
			{
				long addr = num + 16 * i;
				baseItemType2.Tags[i] = base.M.ReadStringU(base.M.Read<long>(addr, new int[1]), 255);
			}
			string[] array = text.Split('/');
			if (array.Length > 3)
			{
				baseItemType2.MoreTagsFromPath = new string[array.Length - 3];
				for (int j = 2; j < array.Length - 1; j++)
				{
					string text2 = Regex.Replace(array[j], "(?<!_)([A-Z])", "_$1").ToLower().Remove(0, 1);
					if (text2[text2.Length - 1] == 's')
					{
						text2 = text2.Remove(text2.Length - 1);
					}
					baseItemType2.MoreTagsFromPath[j - 2] = text2;
				}
			}
			else
			{
				baseItemType2.MoreTagsFromPath = new string[1];
				baseItemType2.MoreTagsFromPath[0] = "";
			}
			ContentsAddr.Add(item, baseItemType2);
			if (!Contents.ContainsKey(text))
			{
				Contents.Add(text, baseItemType2);
			}
		}
	}
}
