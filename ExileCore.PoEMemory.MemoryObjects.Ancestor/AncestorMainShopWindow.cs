using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects.Ancestor;

public class AncestorMainShopWindow : Element
{
	public List<AncestorMainShopWindowOption> Options => GetChildFromIndices(2, 0, 0, 2)?.GetChildrenAs<AncestorMainShopWindowOption>() ?? new List<AncestorMainShopWindowOption>();
}
