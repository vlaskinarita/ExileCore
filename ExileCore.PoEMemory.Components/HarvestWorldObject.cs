using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class HarvestWorldObject : Component
{
	private readonly CachedValue<HarvestWorldObjectComponentOffsets> _cacheValue;

	public List<HarvestSeedSpawnDescriptor> Seeds => base.M.ReadRMOStdVector<HarvestSeedSpawnDescriptor>(_cacheValue.Value.Seeds, 24).ToList();

	public HarvestWorldObject()
	{
		_cacheValue = CreateStructFrameCache<HarvestWorldObjectComponentOffsets>();
	}
}
