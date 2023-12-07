using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Elements;

public class KalandraTabletWindow : Element
{
	public List<TabletTileElement> Tiles => (from x in GetChildFromIndices(2, 0).GetChildrenAs<TabletTileElement>()
		where x.IsVisibleLocal
		select x).ToList();

	public List<TabletChoiceElement> Choices => GetChildFromIndices(3, 0).GetChildrenAs<TabletChoiceElement>().ToList();
}
