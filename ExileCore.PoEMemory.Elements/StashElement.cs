using System;
using System.Collections.Generic;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Elements;

public class StashElement : Element
{
	private readonly CachedValue<StashElementOffsets> _cache;

	public long TotalStashes => StashInventoryPanel?.ChildCount ?? 0;

	public Element ExitButton
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return GetObject<Element>(_cache.Value.ExitButtonPtr);
		}
	}

	public StashTabContainer StashTabContainer
	{
		get
		{
			long num = base.M.Read<long>(_cache.Value.StashTabContainerPtr1 + 632);
			if (num == 0L)
			{
				return GetChildFromIndices(2, 0, 0, 1)?.AsObject<StashTabContainer>();
			}
			return GetObject<StashTabContainer>(num);
		}
	}

	public Element StashTitlePanel
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return GetObject<Element>(_cache.Value.StashTitlePanelPtr);
		}
	}

	public Element StashInventoryPanel
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return StashTabContainer?.StashInventoryPanel;
		}
	}

	public Element ViewAllStashButton
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return StashTabContainer?.ViewAllStashesButton;
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
			return StashTabContainer?.ViewAllStashPanel;
		}
	}

	[Obsolete("Duplicate with ViewAllStashButton")]
	public Element ButtonStashTabListPin
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return StashTabContainer?.ReadObjectAt<Element>(2456);
		}
	}

	public Element PinStashTabListButton
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return StashTabContainer?.PinStashTabListButton;
		}
	}

	public int IndexVisibleStash => StashTabContainer?.VisibleStashIndex ?? 0;

	public Inventory VisibleStash
	{
		get
		{
			if (!base.IsVisible)
			{
				return null;
			}
			return StashTabContainer?.VisibleStash;
		}
	}

	public IList<string> AllStashNames => StashTabContainer?.AllStashNames ?? new List<string>();

	public IList<Inventory> AllInventories => StashTabContainer?.AllInventories ?? new List<Inventory>();

	public IList<Element> TabListButtons => StashTabContainer?.TabListButtons;

	public IList<Element> ViewAllStashPanelChildren => StashTabContainer?.ViewAllStashPanelChildren;

	public StashElement()
	{
		_cache = CreateStructFrameCache<StashElementOffsets>();
	}

	public Inventory GetStashInventoryByIndex(int index)
	{
		return StashTabContainer?.GetStashInventoryByIndex(index);
	}

	public IList<Element> GetTabListButtons()
	{
		return TabListButtons;
	}

	public string GetStashName(int index)
	{
		return StashTabContainer?.GetStashName(index) ?? string.Empty;
	}
}
