using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Elements;

public class StashTopTabSwitcher : Element
{
	public List<Element> SwitchButtons => base.Children.Where((Element x) => x.IsVisible && x.ChildCount > 0).ToList();
}
