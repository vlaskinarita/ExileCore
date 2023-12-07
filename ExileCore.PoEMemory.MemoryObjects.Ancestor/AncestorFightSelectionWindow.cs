using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects.Ancestor;

public class AncestorFightSelectionWindow : Element
{
	public Element TableContainer => base[2];

	public List<AncestorFightSelectionOpponentLine> Options => TableContainer?.GetChildFromIndices(0, 0, 2)?.GetChildrenAs<AncestorFightSelectionOpponentLine>() ?? new List<AncestorFightSelectionOpponentLine>();
}
