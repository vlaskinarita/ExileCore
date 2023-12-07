using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Elements;

public class StashTabContainer : Element
{
	private readonly CachedValue<StashTabContainerOffsets> _cache;

	public int VisibleStashIndex => _cache.Value.VisibleStashIndex;

	public Element StashInventoryPanel
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return base[1];
		}
	}

	public Element ViewAllStashPanel
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return base[4];
		}
	}

	public long TotalStashes => StashInventoryPanel?.ChildCount ?? 0;

	public Element ViewAllStashesButton => GetObject<Element>(_cache.Value.ViewAllStashesButtonPtr);

	public Element PinStashTabListButton => GetObject<Element>(_cache.Value.PinStashTabListButtonPtr);

	public StashTopTabSwitcher TabSwitchBar => GetObject<StashTopTabSwitcher>(_cache.Value.TabSwitchBarPtr);

	public Inventory VisibleStash
	{
		get
		{
			Inventory stashInventoryByIndex = GetStashInventoryByIndex(VisibleStashIndex);
			if (stashInventoryByIndex != null && stashInventoryByIndex.IsVisible)
			{
				return stashInventoryByIndex;
			}
			Inventory inventory = stashInventoryByIndex;
			return AllInventories.FirstOrDefault((Inventory x) => x?.IsVisible ?? false) ?? inventory;
		}
	}

	public IList<Inventory> AllInventories => GetAllInventories();

	public IList<string> AllStashNames => GetAllStashNames();

	public IList<Element> ViewAllStashPanelChildren => ViewAllStashPanel?.Children.LastOrDefault((Element x) => x.ChildCount == TotalStashes)?.Children.Where((Element x) => x.ChildCount > 0).ToList() ?? new List<Element>();

	public IList<Element> TabListButtons => ViewAllStashPanelChildren?.Take((int)TotalStashes).ToList() ?? new List<Element>();

	public StashTabContainer()
	{
		_cache = CreateStructFrameCache<StashTabContainerOffsets>();
	}

	public string GetStashName(int index)
	{
		if (index >= TotalStashes || index < 0)
		{
			return string.Empty;
		}
		return GetStashNameInternal(ViewAllStashPanelChildren, index);
	}

	public virtual Inventory GetStashInventoryByIndex(int index)
	{
		if (index >= TotalStashes)
		{
			return null;
		}
		try
		{
			return StashInventoryPanel?.GetChildAtIndex(index)?.GetChildAtIndex(0)?.AsObject<Inventory>();
		}
		catch
		{
			DebugWindow.LogError($"Not found inventory stash for index: {index}");
			return null;
		}
	}

	private IList<Inventory> GetAllInventories()
	{
		List<Inventory> list = new List<Inventory>();
		for (int i = 0; i < TotalStashes; i++)
		{
			list.Add(GetStashInventoryByIndex(i));
		}
		return list;
	}

	private List<string> GetAllStashNames()
	{
		List<string> list = new List<string>();
		IList<Element> viewAllStashPanelChildren = ViewAllStashPanelChildren;
		for (int i = 0; i < TotalStashes; i++)
		{
			list.Add(GetStashNameInternal(viewAllStashPanelChildren, i));
		}
		return list;
	}

	private static string GetStashNameInternal(IList<Element> viewAllStashPanelChildren, int index)
	{
		return viewAllStashPanelChildren?.ElementAt(index)?.GetChildAtIndex(0).Children?.LastOrDefault()?.Text ?? "";
	}
}
