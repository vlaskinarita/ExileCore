using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.Elements.InventoryElements;

namespace ExileCore.PoEMemory.MemoryObjects;

public class SellWindow : Element
{
	public virtual Element SellDialog => GetChildAtIndex(3);

	public virtual Element YourOffer => SellDialog?.GetChildAtIndex(0);

	public List<NormalInventoryItem> YourOfferItems => YourOffer?.GetChildrenAs<NormalInventoryItem>().Skip(2).ToList() ?? new List<NormalInventoryItem>();

	public virtual Element OtherOffer => SellDialog?.GetChildAtIndex(1);

	public List<NormalInventoryItem> OtherOfferItems => OtherOffer?.GetChildrenAs<NormalInventoryItem>().Skip(1).ToList() ?? new List<NormalInventoryItem>();

	public string NameSeller => SellDialog?.GetChildAtIndex(2)?.Text ?? "";

	public Element AcceptButton => SellDialog?.GetChildAtIndex(5);

	public Element CancelButton => SellDialog?.GetChildAtIndex(6);
}
