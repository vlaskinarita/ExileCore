using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects;

public class BetrayalChoiceAction : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public BetrayalChoice Choice => base.TheGame.Files.BetrayalChoises.GetByAddress(base.M.Read<long>(base.Address + 8));

	public override string ToString()
	{
		return Id + " (" + Choice.Name + ")";
	}
}
