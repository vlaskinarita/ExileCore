using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects.Ancestor;

public class AncestorSideShopPanel : Element
{
	public List<AncestorSidePanelOption> Options => GetChildFromIndices(0, 0, 0, 2)?.GetChildrenAs<AncestorSidePanelOption>() ?? new List<AncestorSidePanelOption>();
}
