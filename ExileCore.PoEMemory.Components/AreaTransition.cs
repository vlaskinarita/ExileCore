using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets.Components;

namespace ExileCore.PoEMemory.Components;

public class AreaTransition : Component
{
	private readonly CachedValue<AreaTransitionComponentOffsets> _cache;

	public int WorldAreaId => _cache.Value.AreaId;

	public WorldArea AreaById => base.TheGame.Files.WorldAreas.GetAreaByAreaId(WorldAreaId);

	public WorldArea WorldArea => base.TheGame.Files.WorldAreas.GetByAddress(_cache.Value.WorldAreaInfoPtr);

	public AreaTransitionType TransitionType => (AreaTransitionType)_cache.Value.TransitionType;

	public AreaTransition()
	{
		_cache = new FrameCache<AreaTransitionComponentOffsets>(() => base.M.Read<AreaTransitionComponentOffsets>(base.Address));
	}
}
