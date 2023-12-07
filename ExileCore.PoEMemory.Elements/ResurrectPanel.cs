namespace ExileCore.PoEMemory.Elements;

public class ResurrectPanel : Element
{
	public Element ResurrectInTown => GetChildFromIndices(1, 0);

	public Element ResurrectAtCheckpoint => GetChildFromIndices(3, 0);
}
