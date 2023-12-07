using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Elements;

public class ArchnemesisPanelElement : Element
{
	public IList<ArchnemesisInventorySlot> InventoryElements => (from x in GetChildFromIndices(2, 0, 0)?.GetChildrenAs<ArchnemesisInventorySlot>()
		where x.HasItem
		select x).ToList();
}
