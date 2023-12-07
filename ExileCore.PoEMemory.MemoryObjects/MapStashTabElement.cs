using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.MemoryObjects;

public class MapStashTabElement : Element
{
	private long mapListStartPtr
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0L;
			}
			return base.M.Read<long>(base.Address + 2520);
		}
	}

	private long mapListEndPtr
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0L;
			}
			return base.M.Read<long>(base.Address + 2520 + 8);
		}
	}

	public int TotalInventories => (int)((mapListEndPtr - mapListStartPtr) / 16);

	public Dictionary<MapSubInventoryKey, MapSubInventoryInfo> MapsCount => GetMapsCount();

	public Dictionary<string, string> MapsCountByName => GetMapsCount2();

	public Dictionary<string, string> MapsCountByTier => GetMapsCountFromUi();

	public Dictionary<string, string> CurrentCell => GetCurrentCell();

	public Dictionary<string, Element> CurrentCellElements => GetCurrentCellElements();

	public Dictionary<string, Element> TierElements => GetTierElements();

	private Dictionary<MapSubInventoryKey, MapSubInventoryInfo> GetMapsCount()
	{
		Dictionary<MapSubInventoryKey, MapSubInventoryInfo> dictionary = new Dictionary<MapSubInventoryKey, MapSubInventoryInfo>();
		MapSubInventoryInfo mapSubInventoryInfo = null;
		MapSubInventoryKey mapSubInventoryKey = null;
		int totalInventories = TotalInventories;
		if (totalInventories > 1024)
		{
			DebugWindow.LogError($"{"MapStashTabElement"}-> {"GetMapsCount"} error {"TotalInventories"} = {totalInventories}");
			return null;
		}
		for (int i = 0; i < totalInventories; i++)
		{
			mapSubInventoryInfo = new MapSubInventoryInfo();
			mapSubInventoryKey = new MapSubInventoryKey();
			mapSubInventoryInfo.Tier = SubInventoryMapTier(i);
			mapSubInventoryInfo.Count = SubInventoryMapCount(i);
			mapSubInventoryInfo.MapName = SubInventoryMapName(i);
			mapSubInventoryKey.Path = SubInventoryMapPath(i);
			mapSubInventoryKey.Type = SubInventoryMapType(i);
			dictionary.Add(mapSubInventoryKey, mapSubInventoryInfo);
		}
		return dictionary;
	}

	private Dictionary<string, string> GetMapsCount2()
	{
		Dictionary<MapSubInventoryKey, MapSubInventoryInfo> mapsCount = GetMapsCount();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<MapSubInventoryKey, MapSubInventoryInfo> item in mapsCount.OrderBy((KeyValuePair<MapSubInventoryKey, MapSubInventoryInfo> x) => x.Value.Tier))
		{
			string value = ((item.Key.Type == MapType.Shaped) ? "Shaped" : "");
			string key = $"{item.Value.Tier}: {value} {item.Value.MapName}";
			string value2 = $"{item.Value.Count}";
			dictionary[key] = value2;
		}
		return dictionary;
	}

	private int SubInventoryMapTier(int index)
	{
		return base.M.Read<int>(mapListStartPtr + index * 16, new int[1]);
	}

	private int SubInventoryMapCount(int index)
	{
		return base.M.Read<int>(mapListStartPtr + index * 16, new int[1] { 8 });
	}

	private MapType SubInventoryMapType(int index)
	{
		return (MapType)base.M.Read<int>(mapListStartPtr + index * 16, new int[1] { 16 });
	}

	private string SubInventoryMapPath(int index)
	{
		return base.M.ReadStringU(base.M.Read<long>(mapListStartPtr + index * 16, new int[2] { 40, 0 }));
	}

	private string SubInventoryMapName(int index)
	{
		return base.M.ReadStringU(base.M.Read<long>(mapListStartPtr + index * 16, new int[2] { 40, 32 }));
	}

	private Dictionary<string, Element> GetCurrentCellElements()
	{
		IList<Element> children = base.Children[2].Children[0].Children[0].Children;
		Dictionary<string, Element> dictionary = new Dictionary<string, Element>();
		foreach (Element item in children)
		{
			string text = item?.Tooltip?.Children?[0].Children[0].Children[3].Text;
			if (text == null)
			{
				string text2 = item.Tooltip?.Text;
				text = ((text2 == null) ? "Error" : text2.Substring(0, text2.IndexOf('\n')));
			}
			_ = item.Children[4].Text;
			dictionary.Add(text, item);
		}
		return dictionary;
	}

	private Dictionary<string, string> GetCurrentCell()
	{
		IList<Element> children = base.Children[2].Children[0].Children[0].Children;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (Element item in children)
		{
			string text = item?.Tooltip?.Children?[0].Children[0].Children[3].Text;
			if (text == null)
			{
				string text2 = item.Tooltip?.Text;
				text = ((text2 == null) ? "Error" : text2.Substring(0, text2.IndexOf('\n')));
			}
			string text3 = item.Children[4].Text;
			dictionary.Add(text, text3);
		}
		return dictionary;
	}

	private Dictionary<string, string> GetMapsCountFromUi()
	{
		IEnumerable<Element> enumerable = base.Children[0].Children.Concat(base.Children[1].Children);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (Element item in enumerable)
		{
			dictionary.Add(item.Children[0].Text, item.Children[1].Text);
		}
		return dictionary;
	}

	private Dictionary<string, Element> GetTierElements()
	{
		IEnumerable<Element> enumerable = base.Children[0].Children.Concat(base.Children[1].Children);
		Dictionary<string, Element> dictionary = new Dictionary<string, Element>();
		int num = 1;
		foreach (Element item in enumerable)
		{
			string text = item.Children[0].Text;
			if (text != "U")
			{
				text = num.ToString();
				num++;
			}
			dictionary[text] = item;
		}
		return dictionary;
	}
}
