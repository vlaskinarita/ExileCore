using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.Elements;

public class TabletChoiceElement : Element
{
	private StampChoice _choice;

	public StampChoice Choice => _choice ?? (_choice = base.TheGame.Files.StampChoices.GetByAddress(base.M.Read<long>(base.Address + 792 - 8)));
}
