using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Elements.Sanctum;

public class SanctumFloorWindow : Element
{
	private readonly CachedValue<SanctumFloorWindowOffsets> _cachedValue;

	public List<List<SanctumRoomElement>> RoomsByLayer => GetChildFromIndices(0, 0, 0, 1)?.Children.Select((Element x) => x.GetChildrenAs<SanctumRoomElement>()).ToList() ?? new List<List<SanctumRoomElement>>();

	public List<SanctumRoomElement> Rooms => RoomsByLayer.SelectMany((List<SanctumRoomElement> x) => x).ToList();

	private SanctumFloorWindowDataSelector DataSelector
	{
		get
		{
			SanctumFloorWindowOffsets value = _cachedValue.Value;
			long inSanctumDataPtr = value.InSanctumDataPtr;
			(bool, long) tuple;
			if (inSanctumDataPtr == 0L)
			{
				long outOfSanctumDataPtr = value.OutOfSanctumDataPtr;
				tuple = ((outOfSanctumDataPtr == 0L) ? (false, 0L) : (true, outOfSanctumDataPtr));
			}
			else
			{
				tuple = (false, inSanctumDataPtr);
			}
			(bool, long) tuple2 = tuple;
			bool item = tuple2.Item1;
			long item2 = tuple2.Item2;
			SanctumFloorWindowDataSelector @object = GetObject<SanctumFloorWindowDataSelector>(item2);
			@object.IsOutsidePtr = item;
			return @object;
		}
	}

	public SanctumFloorData FloorData => DataSelector.FloorData;

	public SanctumFloorWindow()
	{
		_cachedValue = CreateStructFrameCache<SanctumFloorWindowOffsets>();
	}
}
