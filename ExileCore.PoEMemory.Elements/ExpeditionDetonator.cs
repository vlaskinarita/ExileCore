namespace ExileCore.PoEMemory.Elements;

public class ExpeditionDetonator : Element
{
	public ExpeditionDetonatorInfo Info => ReadObjectAt<ExpeditionDetonatorInfo>(632);

	public int RemainingExplosives => int.Parse(GetChildFromIndices(default(int), default(int), default(int)).Text);

	public Element RevertExplosiveButton => GetChildFromIndices(0, 0, 1);

	public Element ToggleExplosivePlacementButton => GetChildFromIndices(0, 1);
}
