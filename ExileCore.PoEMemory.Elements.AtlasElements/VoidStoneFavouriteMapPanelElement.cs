namespace ExileCore.PoEMemory.Elements.AtlasElements;

public class VoidStoneFavouriteMapPanelElement : Element
{
	public Element InfoHoverIcon => GetChildAtIndex(0);

	public Element FavouriteMapSlots => GetChildAtIndex(1);

	public VoidStoneInventory VoidStoneInventory => base[2].AsObject<VoidStoneInventory>();
}
