using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements;

public class MapReceptacleWindow : Element
{
	public Element CloseMapDialog => GetChildAtIndex(3);

	public Element ActivateButton => GetChildFromIndices(4);

	public Element MapPiecesPanel => GetChildAtIndex(7);

	public List<Element> MapsElements => MapPiecesPanel.Children.Where((Element x) => x.Entity?.IsValid ?? false).ToList();

	public List<Entity> InsertedMaps => MapsElements.Select((Element x) => x.Entity).ToList();
}
