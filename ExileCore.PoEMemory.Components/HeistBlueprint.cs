using System.Collections.Generic;
using ExileCore.PoEMemory.MemoryObjects.Heist;
using ExileCore.Shared.Cache;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class HeistBlueprint : Component
{
	public class Wing : RemoteMemoryObject
	{
		public List<(HeistJobRecord, int)> Jobs => GetJobs(base.Address);

		public List<HeistChestRewardTypeRecord> RewardRooms => GetRooms(base.Address + 32);

		public List<HeistNpcRecord> Crew => GetCrew(base.Address + 56);

		private List<(HeistJobRecord, int)> GetJobs(long source)
		{
			List<(HeistJobRecord, int)> list = new List<(HeistJobRecord, int)>();
			long num = base.M.Read<long>(source);
			long num2 = base.M.Read<long>(source + 8);
			int jobRecordSize = HeistBlueprintComponentOffsets.JobRecordSize;
			int num3 = 0;
			long num4 = num;
			while (num4 < num2 && num3 < 50)
			{
				long num5 = base.M.Read<long>(num4);
				if (num5 != 0L)
				{
					byte item = base.M.Read<byte>(num4 + 16);
					HeistJobRecord byAddress = base.TheGame.Files.HeistJobs.GetByAddress(num5);
					if (byAddress != null)
					{
						list.Add((byAddress, item));
					}
				}
				num4 += jobRecordSize;
				num3++;
			}
			return list;
		}

		private List<HeistChestRewardTypeRecord> GetRooms(long source)
		{
			List<HeistChestRewardTypeRecord> list = new List<HeistChestRewardTypeRecord>();
			long num = base.M.Read<long>(source);
			long num2 = base.M.Read<long>(source + 8);
			int roomRecordSize = HeistBlueprintComponentOffsets.RoomRecordSize;
			int num3 = 0;
			long num4 = num;
			while (num4 < num2 && num3 < 50)
			{
				long num5 = base.M.Read<long>(num4);
				if (num5 != 0L)
				{
					HeistChestRewardTypeRecord byAddress = base.TheGame.Files.HeistChestRewardType.GetByAddress(num5);
					if (byAddress != null)
					{
						list.Add(byAddress);
					}
				}
				num4 += roomRecordSize;
				num3++;
			}
			return list;
		}

		private List<HeistNpcRecord> GetCrew(long source)
		{
			List<HeistNpcRecord> list = new List<HeistNpcRecord>();
			long num = base.M.Read<long>(source);
			long num2 = base.M.Read<long>(source + 8);
			int npcRecordSize = HeistBlueprintComponentOffsets.NpcRecordSize;
			int num3 = 0;
			long num4 = num;
			while (num4 < num2 && num3 < 50)
			{
				long num5 = base.M.Read<long>(num4 + 8);
				if (num5 != 0L)
				{
					HeistNpcRecord byAddress = base.TheGame.Files.HeistNpcs.GetByAddress(num5);
					if (byAddress != null)
					{
						list.Add(byAddress);
					}
				}
				num4 += npcRecordSize;
				num3++;
			}
			return list;
		}
	}

	private readonly CachedValue<HeistBlueprintComponentOffsets> _CachedBlueprint;

	public HeistBlueprintComponentOffsets BlueprintStruct => _CachedBlueprint.Value;

	public byte AreaLevel => BlueprintStruct.AreaLevel;

	public bool IsConfirmed => BlueprintStruct.IsConfirmed == 1;

	public List<Wing> Wings => GetWings(BlueprintStruct.Wings);

	public HeistBlueprint()
	{
		_CachedBlueprint = new FrameCache<HeistBlueprintComponentOffsets>(() => base.M.Read<HeistBlueprintComponentOffsets>(base.Address));
	}

	private List<Wing> GetWings(NativePtrArray source)
	{
		List<Wing> list = new List<Wing>();
		int wingRecordSize = HeistBlueprintComponentOffsets.WingRecordSize;
		int num = 0;
		long num2 = source.First;
		while (num2 < source.Last && num < 50)
		{
			Wing @object = GetObject<Wing>(num2);
			if (@object != null)
			{
				list.Add(@object);
			}
			num2 += wingRecordSize;
			num++;
		}
		return list;
	}
}
