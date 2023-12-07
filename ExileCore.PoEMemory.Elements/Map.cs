namespace ExileCore.PoEMemory.Elements;

public class Map : Element
{
	private SubMap _largeMap;

	private SubMap _smallMap;

	public SubMap LargeMap => _largeMap ?? (_largeMap = ReadObjectAt<SubMap>(640));

	public float LargeMapShiftX => base.M.Read<float>(LargeMap.Address + 616);

	public float LargeMapShiftY => base.M.Read<float>(LargeMap.Address + 620);

	public float LargeMapZoom => base.M.Read<float>(LargeMap.Address + 684);

	public SubMap SmallMiniMap => _smallMap ?? (_smallMap = ReadObjectAt<SubMap>(648));

	public float SmallMinMapX => base.M.Read<float>(SmallMiniMap.Address + 616);

	public float SmallMinMapY => base.M.Read<float>(SmallMiniMap.Address + 620);

	public float SmallMinMapZoom => base.M.Read<float>(SmallMiniMap.Address + 684);

	public Element OrangeWords => ReadObjectAt<Element>(680);

	public Element BlueWords => ReadObjectAt<Element>(848);
}
