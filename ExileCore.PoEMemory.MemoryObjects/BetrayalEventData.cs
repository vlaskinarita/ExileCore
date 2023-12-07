using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects;

public class BetrayalEventData : Element
{
	public BetrayalTarget Target1 => base.TheGame.Files.BetrayalTargets.GetByAddress(base.M.Read<long>(base.Address + 784));

	public BetrayalTarget Target2 => base.TheGame.Files.BetrayalTargets.GetByAddress(base.M.Read<long>(base.Address + 816));

	public BetrayalTarget Target3 => base.TheGame.Files.BetrayalTargets.GetByAddress(base.M.Read<long>(base.Address + 832));

	public BetrayalChoiceAction Action => base.TheGame.Files.BetrayalChoiceActions.GetByAddress(base.M.Read<long>(base.Address + 800));

	public string EventText => GetChildFromIndices(8, 1)?.Text;

	public Element ReleaseButton => base[6];

	public Element InterrogateButton => GetChildFromIndices(7, 0);

	public Element SpecialButton => GetChildFromIndices(8, 0);
}
