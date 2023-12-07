using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Elements.Sanctum;

public class SanctumFloorWindowDataSelector : RemoteMemoryObject
{
	private readonly CachedValue<SanctumFloorWindowDataOffsets> _cachedValue;

	public bool IsOutsidePtr { get; set; }

	public SanctumFloorData FloorData
	{
		get
		{
			long address = base.Address;
			SanctumFloorWindowDataOffsets value = _cachedValue.Value;
			bool isOutsidePtr = IsOutsidePtr;
			long num = ((value.Flag1 && !isOutsidePtr) ? ((!value.Flag2) ? 408 : 480) : 352);
			return GetObject<SanctumFloorData>(address + num);
		}
	}

	public SanctumFloorWindowDataSelector()
	{
		_cachedValue = CreateStructFrameCache<SanctumFloorWindowDataOffsets>();
	}
}
