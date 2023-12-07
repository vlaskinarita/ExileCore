using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Elements;

public class HarvestWindow : Element
{
	public List<HarvestCraftElement> Crafts => GetChildFromIndices(8, 0, 1).Children.Select((Element x) => x.GetChildAtIndex(3).AsObject<HarvestCraftElement>()).ToList();
}
