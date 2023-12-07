using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class MapDeviceWindow : Element
{
	private readonly CachedValue<MapDeviceWindowOffsets> _cachedValue;

	public Element OpenMapDialog => GetChildAtIndex(3);

	public Element CloseMapDialog => GetChildAtIndex(4);

	public Element ChooseZanaMod => GetChildAtIndex(5);

	public Element BottomMapSettings => GetObject<Element>(_cachedValue.Value.BottomPanelPtr);

	public Element ActivateButton => BottomMapSettings?.GetChildAtIndex(1);

	public Element ChooseMastersMods => BottomMapSettings?.GetChildAtIndex(3);

	public List<NormalInventoryItem> Items => BottomMapSettings?.Children.Skip(7).Take(6).SelectMany((Element x) => x.GetChildrenAs<NormalInventoryItem>().Skip(1))
		.ToList() ?? new List<NormalInventoryItem>();

	public MapDeviceWindow()
	{
		_cachedValue = new FrameCache<MapDeviceWindowOffsets>(() => base.M.Read<MapDeviceWindowOffsets>(base.Address));
	}
}
