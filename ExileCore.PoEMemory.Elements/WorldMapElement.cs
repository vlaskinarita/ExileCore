namespace ExileCore.PoEMemory.Elements;

public class WorldMapElement : Element
{
	public Element Panel => GetObject<Element>(base.M.Read<long>(base.Address + 2744, new int[1] { 3088 }));

	public Element WaypointsRoot => GetChildFromIndices(2, 0, 1, 0, 0, 1, 1, 0, 0, 2);

	public Element GetPartButton(int part)
	{
		if (part >= 1 && part <= 2)
		{
			return FindChildRecursive("Part " + part);
		}
		if (part == 3)
		{
			return FindChildRecursive("Epilogue");
		}
		return null;
	}

	public Element GetActButton(int act)
	{
		Element childFromIndices = GetChildFromIndices(2, 0, 1);
		if (act >= 1 && act <= 10)
		{
			return childFromIndices?.FindChildRecursive("Act " + act);
		}
		if (act == 11)
		{
			return childFromIndices?.FindChildRecursive("Epilogue");
		}
		return null;
	}
}
