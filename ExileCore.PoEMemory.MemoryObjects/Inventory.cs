using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class Inventory : Element
{
	private readonly CachedValue<InventoryOffsets> _cachedValue;

	private readonly CachedValue<Inventory> _nestedInventory;

	private readonly CachedValue<bool> _isNestedInventory;

	private InventoryType _cacheInventoryType;

	protected virtual Element OffsetContainerElement => GetChildAtIndex(0);

	private InventoryOffsets InventoryStruct => NestedVisibleInventory?.InventoryStruct ?? _cachedValue.Value;

	public ServerInventory ServerInventory => GetServerInventory();

	public long ItemCount => InventoryStruct.ItemCount;

	public long TotalBoxesInInventoryRow => InventoryStruct.InventorySize.X;

	public NormalInventoryItem HoverItem
	{
		get
		{
			if (InventoryStruct.HoverItem != 0L)
			{
				return GetObject<NormalInventoryItem>(InventoryStruct.HoverItem);
			}
			return null;
		}
	}

	public new int X => InventoryStruct.RealPos.X;

	public new int Y => InventoryStruct.RealPos.Y;

	public int XFake => InventoryStruct.FakePos.X;

	public int YFake => InventoryStruct.FakePos.Y;

	public bool CursorHoverInventory => InventoryStruct.CursorInInventory == 1;

	public bool IsNestedInventory => _isNestedInventory.Value;

	public InventoryType InvType => GetInvType();

	public Element InventoryUIElement
	{
		get
		{
			if (!IsNestedInventory)
			{
				return getInventoryElement();
			}
			return NestedVisibleInventory?.InventoryUIElement;
		}
	}

	private Inventory NestedVisibleInventory => _nestedInventory.Value;

	private StashTabContainer NestedStashContainer => base[1]?.AsObject<StashTabContainer>();

	public int? NestedVisibleInventoryIndex
	{
		get
		{
			if (!IsNestedInventory)
			{
				return null;
			}
			return NestedStashContainer?.VisibleStashIndex;
		}
	}

	public StashTopTabSwitcher NestedTabSwitchBar
	{
		get
		{
			if (!IsNestedInventory)
			{
				return null;
			}
			return NestedStashContainer?.TabSwitchBar;
		}
	}

	public IList<NormalInventoryItem> VisibleInventoryItems
	{
		get
		{
			if (IsNestedInventory)
			{
				return NestedVisibleInventory?.VisibleInventoryItems ?? new List<NormalInventoryItem>();
			}
			Element inventoryUIElement = InventoryUIElement;
			if (inventoryUIElement == null || inventoryUIElement.Address == 0L)
			{
				return null;
			}
			List<NormalInventoryItem> list = new List<NormalInventoryItem>();
			switch (InvType)
			{
			case InventoryType.PlayerInventory:
			case InventoryType.NormalStash:
			case InventoryType.QuadStash:
			case InventoryType.VendorInventory:
				list.AddRange(inventoryUIElement.GetChildrenAs<NormalInventoryItem>().Skip(1));
				break;
			case InventoryType.CurrencyStash:
			{
				Element element2 = null;
				if (base.Children[1].IsVisible)
				{
					element2 = base.Children[1];
				}
				else if (base.Children[2].IsVisible)
				{
					element2 = base.Children[2];
				}
				if (element2 != null)
				{
					foreach (Element child in element2.Children)
					{
						if (child.ChildCount > 1)
						{
							list.Add(child[1].AsObject<EssenceInventoryItem>());
						}
					}
				}
				foreach (Element child2 in inventoryUIElement.Children)
				{
					if (child2.ChildCount > 1)
					{
						list.Add(child2[1].AsObject<EssenceInventoryItem>());
					}
				}
				break;
			}
			case InventoryType.EssenceStash:
				foreach (Element child3 in inventoryUIElement.Children)
				{
					if (child3.ChildCount > 1)
					{
						list.Add(child3[1].AsObject<EssenceInventoryItem>());
					}
				}
				break;
			case InventoryType.FragmentStash:
			{
				Element element = null;
				if (base.Children[0].IsVisible)
				{
					element = base.Children[0];
				}
				else if (base.Children[1].IsVisible)
				{
					element = base.Children[1];
				}
				else if (base.Children[2].IsVisible)
				{
					element = base.Children[2];
				}
				else if (base.Children[3].IsVisible)
				{
					element = null;
				}
				if (element == null)
				{
					break;
				}
				foreach (Element child4 in element.Children)
				{
					if (child4.ChildCount > 1)
					{
						list.Add(child4[1].AsObject<FragmentInventoryItem>());
					}
				}
				break;
			}
			case InventoryType.DivinationStash:
				return (from x in inventoryUIElement.GetChildAtIndex(0)?.GetChildAtIndex(1)?.Children.Where((Element x) => x != null && x.IsVisibleLocal && x.ChildCount > 1 && x.GetChildAtIndex(1).ChildCount > 1).Select((Func<Element, NormalInventoryItem>)((Element x) => x.GetChildAtIndex(1)?.GetChildAtIndex(1)?.AsObject<DivinationInventoryItem>()))
					where x != null
					select x).ToList() ?? list;
			case InventoryType.MapStash:
				if (base.ChildCount <= 3)
				{
					break;
				}
				foreach (Element item in base[3].Children.Where((Element x) => x.IsVisible))
				{
					foreach (NormalInventoryItem item2 in item.GetChildrenAs<NormalInventoryItem>().Skip(1))
					{
						list.Add(item2);
					}
				}
				list.Sort((NormalInventoryItem i1, NormalInventoryItem i2) => (i1.PositionNum.X * 6f + i1.PositionNum.Y).CompareTo(i2.PositionNum.X * 6f + i2.PositionNum.Y));
				break;
			case InventoryType.DelveStash:
				foreach (Element child5 in inventoryUIElement.Children)
				{
					if (child5.ChildCount > 1)
					{
						list.Add(child5[1].AsObject<DelveInventoryItem>());
					}
				}
				break;
			case InventoryType.BlightStash:
				foreach (Element child6 in inventoryUIElement.Children)
				{
					if (child6.ChildCount > 1)
					{
						list.Add(child6[1].AsObject<BlightInventoryItem>());
					}
				}
				break;
			case InventoryType.DeliriumStash:
				foreach (Element child7 in inventoryUIElement.Children)
				{
					if (child7.ChildCount > 1)
					{
						list.Add(child7[1].AsObject<DeliriumInventoryItem>());
					}
				}
				break;
			case InventoryType.MetamorphStash:
				foreach (Element child8 in inventoryUIElement.Children)
				{
					if (child8.ChildCount > 1)
					{
						list.Add(child8[1].AsObject<MetamorphInventoryItem>());
					}
				}
				break;
			case InventoryType.UniqueStash:
				foreach (Element child9 in inventoryUIElement.Children)
				{
					if (child9.ChildCount > 1)
					{
						list.Add(child9[1].AsObject<NormalInventoryItem>());
					}
				}
				break;
			case InventoryType.GemStash:
				return (from x in inventoryUIElement.Children.Where((Element subInventory) => subInventory.IsVisibleLocal).SelectMany(delegate(Element x)
					{
						IEnumerable<Element> enumerable = x[1]?.Children;
						return enumerable ?? Enumerable.Empty<Element>();
					}).Select((Func<Element, NormalInventoryItem>)((Element slot) => slot[1]?.AsObject<GemInventoryItem>()))
					where x != null
					select x).ToList();
			case InventoryType.FlaskStash:
				return (from x in inventoryUIElement.Children.Where((Element subInventory) => subInventory.IsVisibleLocal).SelectMany(delegate(Element x)
					{
						IEnumerable<Element> enumerable2 = x[1]?.Children;
						return enumerable2 ?? Enumerable.Empty<Element>();
					}).Select((Func<Element, NormalInventoryItem>)((Element slot) => slot[1]?.AsObject<FlaskInventoryItem>()))
					where x != null
					select x).ToList();
			}
			return list;
		}
	}

	public Entity this[int x, int y, int xLength]
	{
		get
		{
			long num = base.M.Read<long>(base.Address + 1040, new int[2] { 1600, 56 });
			y *= xLength;
			long num2 = base.M.Read<long>(num + (x + y) * 8);
			if (num2 <= 0)
			{
				return null;
			}
			return ReadObject<Entity>(num2);
		}
	}

	public Inventory()
	{
		_cachedValue = new FrameCache<InventoryOffsets>(() => base.M.Read<InventoryOffsets>(OffsetContainerElement?.Address ?? 0));
		_nestedInventory = new FrameCache<Inventory>(() => (!IsNestedInventory) ? null : GetNestedVisibleInventory());
		_isNestedInventory = new FrameCache<bool>(() => base.ChildCount == 2 && base[1].ChildCount == 5);
	}

	protected virtual InventoryType GetInvType()
	{
		if (IsNestedInventory)
		{
			return NestedVisibleInventory?.InvType ?? InventoryType.InvalidInventory;
		}
		if (_cacheInventoryType != 0)
		{
			return _cacheInventoryType;
		}
		if (base.Address == 0L)
		{
			return InventoryType.InvalidInventory;
		}
		for (int i = 1; i < InventoryList.InventoryCount; i++)
		{
			if (base.TheGame.IngameState.IngameUi.InventoryPanel[(InventoryIndex)i].Address == base.Address)
			{
				_cacheInventoryType = InventoryType.PlayerInventory;
				return _cacheInventoryType;
			}
		}
		long childCount = base.ChildCount;
		if (childCount <= 18)
		{
			long num = childCount - 1;
			if ((ulong)num <= 8uL)
			{
				switch (num)
				{
				case 4L:
					goto IL_0117;
				case 6L:
					goto IL_0144;
				case 0L:
					goto IL_0150;
				case 8L:
					goto IL_019e;
				case 3L:
					goto IL_01a8;
				case 1L:
				case 2L:
				case 5L:
				case 7L:
					goto IL_01ff;
				}
			}
			if (childCount != 17)
			{
				if (childCount != 18)
				{
					goto IL_01ff;
				}
				_cacheInventoryType = InventoryType.CurrencyStash;
			}
			else
			{
				_cacheInventoryType = InventoryType.MetamorphStash;
			}
		}
		else if (childCount <= 84)
		{
			if (childCount != 35)
			{
				if (childCount != 84)
				{
					goto IL_01ff;
				}
				_cacheInventoryType = InventoryType.BlightStash;
			}
			else
			{
				_cacheInventoryType = InventoryType.DelveStash;
			}
		}
		else if (childCount != 88)
		{
			if (childCount != 111)
			{
				goto IL_01ff;
			}
			_cacheInventoryType = InventoryType.EssenceStash;
		}
		else
		{
			_cacheInventoryType = InventoryType.DeliriumStash;
		}
		goto IL_0206;
		IL_0144:
		_cacheInventoryType = InventoryType.MapStash;
		goto IL_0206;
		IL_01ff:
		_cacheInventoryType = InventoryType.InvalidInventory;
		goto IL_0206;
		IL_0206:
		return _cacheInventoryType;
		IL_0150:
		if (TotalBoxesInInventoryRow == 24)
		{
			_cacheInventoryType = InventoryType.QuadStash;
		}
		else
		{
			_cacheInventoryType = InventoryType.NormalStash;
		}
		goto IL_0206;
		IL_019e:
		_cacheInventoryType = InventoryType.UniqueStash;
		goto IL_0206;
		IL_01a8:
		_cacheInventoryType = base[0]?.ChildCount switch
		{
			4L => InventoryType.GemStash, 
			5L => InventoryType.FlaskStash, 
			_ => InventoryType.InvalidInventory, 
		};
		goto IL_0206;
		IL_0117:
		if (base.Children[4].ChildCount == 4)
		{
			_cacheInventoryType = InventoryType.FragmentStash;
		}
		else
		{
			_cacheInventoryType = InventoryType.DivinationStash;
		}
		goto IL_0206;
	}

	private Element getInventoryElement()
	{
		switch (InvType)
		{
		case InventoryType.PlayerInventory:
		case InventoryType.CurrencyStash:
		case InventoryType.EssenceStash:
		case InventoryType.DivinationStash:
		case InventoryType.FragmentStash:
		case InventoryType.DelveStash:
		case InventoryType.BlightStash:
		case InventoryType.DeliriumStash:
		case InventoryType.MetamorphStash:
		case InventoryType.VendorInventory:
			return this;
		case InventoryType.NormalStash:
		case InventoryType.QuadStash:
			return GetChildAtIndex(0);
		case InventoryType.FlaskStash:
		case InventoryType.GemStash:
			return GetChildFromIndices(1, 0, 1);
		case InventoryType.MapStash:
			return AsObject<Element>().Parent?.AsObject<MapStashTabElement>();
		case InventoryType.UniqueStash:
			return AsObject<Element>().Parent;
		default:
			return null;
		}
	}

	private ServerInventory GetServerInventory()
	{
		switch (InvType)
		{
		case InventoryType.PlayerInventory:
		case InventoryType.NormalStash:
		case InventoryType.QuadStash:
		case InventoryType.VendorInventory:
			return InventoryUIElement?.ReadObjectAt<ServerInventory>(1248);
		case InventoryType.DivinationStash:
			return InventoryUIElement?.ReadObjectAt<ServerInventory>(872);
		case InventoryType.BlightStash:
			return InventoryUIElement?.ReadObjectAt<Element>(960).ReadObjectAt<ServerInventory>(1248);
		case InventoryType.CurrencyStash:
		case InventoryType.EssenceStash:
		case InventoryType.MetamorphStash:
			return InventoryUIElement?.ReadObjectAt<Element>(96).ReadObjectAt<ServerInventory>(1248);
		default:
			return null;
		}
	}

	private Inventory GetNestedVisibleInventory()
	{
		return NestedStashContainer?.VisibleStash;
	}
}
