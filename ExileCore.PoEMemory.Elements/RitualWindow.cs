using System.Collections.Generic;
using ExileCore.PoEMemory.Elements.InventoryElements;

namespace ExileCore.PoEMemory.Elements;

public class RitualWindow : Element
{
	public Element InventoryElement => ReadObjectAt<Element>(704);

	public List<NormalInventoryItem> Items => InventoryElement.GetChildrenAs<NormalInventoryItem>();
}
