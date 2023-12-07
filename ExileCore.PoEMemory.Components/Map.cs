using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class Map : Component
{
	private readonly Lazy<MapComponentBase> mapBase;

	private readonly Lazy<MapComponentInner> mapInner;

	private readonly CachedValue<WorldArea> _area;

	public MapComponentInner MapInformation => mapInner.Value;

	public WorldArea Area => _area.Value;

	public byte Tier => mapBase.Value.Tier;

	public InventoryTabMapSeries MapSeries => (InventoryTabMapSeries)MapInformation.MapSeries;

	public Map()
	{
		mapBase = new Lazy<MapComponentBase>(() => base.M.Read<MapComponentBase>(base.Address));
		mapInner = new Lazy<MapComponentInner>(() => base.M.Read<MapComponentInner>(mapBase.Value.Base));
		_area = new StaticValueCache<WorldArea>(() => base.TheGame.Files.WorldAreas.GetByAddress(MapInformation.Area));
	}
}
