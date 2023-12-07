using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class Targetable : Component
{
	private readonly CachedValue<TargetableComponentOffsets> _cachedValue;

	public TargetableComponentOffsets TargetableComponent => _cachedValue.Value;

	public bool isTargetable => TargetableComponent.isTargetable;

	public bool isTargeted => TargetableComponent.isTargeted;

	public Targetable()
	{
		_cachedValue = new FrameCache<TargetableComponentOffsets>(() => base.M.Read<TargetableComponentOffsets>(base.Address));
	}
}
