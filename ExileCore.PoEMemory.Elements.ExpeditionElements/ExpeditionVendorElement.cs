using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements.ExpeditionElements;

public class ExpeditionVendorElement : Element
{
	public SearchBarElement HighlightSearchbarElement => GetChildFromIndices(4, 3).AsObject<SearchBarElement>();

	public Element VendorResponseTextBox => GetChildFromIndices(6, 1);

	public string VendorWindowTitle => GetChildFromIndices(6, 2, 0).Text;

	public Element RefreshItemsButton => GetChildFromIndices(7, 0);

	public Element RefreshCurrencyInfoBox => GetChildFromIndices(7, 1);

	public List<NormalInventoryItem> InventoryItems => GetChildFromIndices(8, 1, 0, 0).GetChildrenAs<NormalInventoryItem>().Skip(1).ToList();

	public ExpeditionVendorCurrencyInfoElement CurrencyInfo => base[9]?.AsObject<ExpeditionVendorCurrencyInfoElement>();

	public List<Entity> OfferedItems => InventoryItems.Select((NormalInventoryItem elem) => elem.Item).ToList();

	public TujenHaggleWindowElement TujenHaggleWindow => GetChildFromIndices(11, 0).AsObject<TujenHaggleWindowElement>();
}
