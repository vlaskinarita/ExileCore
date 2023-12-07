namespace ExileCore.PoEMemory.MemoryObjects;

public class CraftBenchWindow : Element
{
	public Element PrefixesElement => GetChildFromIndices(3, 0, 0, 1, 0);

	public Element SuffixesElement => GetChildFromIndices(3, 0, 1, 1, 0);

	public Element FilterElement => GetChildFromIndices(2, 1, 0, 0);

	public Element CraftsListElement => GetChildFromIndices(3);

	public Element ItemSlotElement => GetChildFromIndices(5, 1);

	public Element CraftButton => GetChildFromIndices(5, 0, 0);
}
