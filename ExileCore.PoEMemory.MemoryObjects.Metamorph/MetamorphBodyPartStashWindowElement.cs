using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects.Metamorph;

public class MetamorphBodyPartStashWindowElement : Element
{
	public string BodyPartName => GetChildFromIndices(1, 0).Text;

	public IEnumerable<MetamorphBodyPartElement> GetBodyPartStashWindowElements => GetChildAtIndex(0).GetChildrenAs<MetamorphBodyPartElement>();

	public override string ToString()
	{
		return BodyPartName ?? "";
	}
}
