using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Elements;

public class ArchnemesisAltarElement : Element
{
	public IList<ArchnemesisAltarInventorySlot> InventoryElements => (from x in GetChildFromIndices(2, 0)?.GetChildrenAs<ArchnemesisAltarInventorySlot>()
		where x.HasItem
		select x).ToList();
}
