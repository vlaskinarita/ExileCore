using ExileCore.PoEMemory.Elements.InventoryElements;

namespace ExileCore.PoEMemory.MemoryObjects;

public class CardTradeWindow : Element
{
	public Element CardSlotElement => GetChildAtIndex(5);

	public NormalInventoryItem CardSlotItem => CardSlotElement.GetChildFromIndices(1)?.AsObject<NormalInventoryItem>();

	public Element TradeButton => GetChildAtIndex(4);
}
