using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects.Metamorph;

public class MetamorphWindowElement : Element
{
	public Element MetamorphStash => ReadObjectAt<Element>(544);

	public IEnumerable<MetamorphBodyPartStashWindowElement> GetBodyPartStashWindowElements => MetamorphStash.GetChildrenAs<MetamorphBodyPartStashWindowElement>();
}
