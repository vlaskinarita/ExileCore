namespace ExileCore.PoEMemory.Components;

public class WorldDescription : Component
{
	public Element Element => ReadObjectAt<Element>(40);
}
