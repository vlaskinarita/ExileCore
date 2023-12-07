using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class ExpeditionSaga : Component
{
	private readonly CachedValue<ExpeditionSagaOffsets> _cachedValue;

	public ExpeditionSagaOffsets SagaStruct => _cachedValue.Value;

	public int AreaLevel => SagaStruct.AreaLevel;

	public List<ExpeditionAreaData> Areas
	{
		get
		{
			long first = SagaStruct.AreasData.First;
			long last = SagaStruct.AreasData.Last;
			if (first == 0L || (last - first) / 192 > 1024)
			{
				return new List<ExpeditionAreaData>();
			}
			return base.M.ReadStructsArray<ExpeditionAreaData>(first, last, 192, this).ToList();
		}
	}

	public ExpeditionSaga()
	{
		_cachedValue = new FrameCache<ExpeditionSagaOffsets>(() => base.M.Read<ExpeditionSagaOffsets>(base.Address));
	}
}
