using System;
using ExileCore.PoEMemory.MemoryObjects.Heist;
using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class HeistEquipment : Component
{
	private readonly Lazy<HeistEquipmentOffsets> _HeistEquipmentItem;

	private readonly CachedValue<BaseItemType> _ItemBase;

	private readonly CachedValue<HeistJobRecord> _Job;

	public BaseItemType ItemBase => _ItemBase.Value;

	public HeistJobRecord RequiredJob => _Job.Value;

	public int JobMinimumLevel => _HeistEquipmentItem.Value.RequiredJobMinimumLevel;

	public HeistJobE RequiredJobE
	{
		get
		{
			if (_Job.Value != null)
			{
				return (HeistJobE)base.TheGame.Files.HeistJobs.EntriesList.FindIndex((HeistJobRecord job) => job.Address == _HeistEquipmentItem.Value.RequiredJobKey);
			}
			return HeistJobE.Any;
		}
	}

	public HeistEquipment()
	{
		Lazy<HeistEquipmentComponentOffsets> component = new Lazy<HeistEquipmentComponentOffsets>(() => base.M.Read<HeistEquipmentComponentOffsets>(base.Address));
		Lazy<HeistEquipmentComponentDataOffsets> componentData = new Lazy<HeistEquipmentComponentDataOffsets>(() => base.M.Read<HeistEquipmentComponentDataOffsets>(component.Value.DataKey));
		_HeistEquipmentItem = new Lazy<HeistEquipmentOffsets>(() => base.M.Read<HeistEquipmentOffsets>(componentData.Value.HeistEquipmentKey));
		_ItemBase = new StaticValueCache<BaseItemType>(() => base.TheGame.Files.BaseItemTypes.GetFromAddress(_HeistEquipmentItem.Value.BaseItemKey));
		_Job = new StaticValueCache<HeistJobRecord>(() => base.TheGame.Files.HeistJobs.GetByAddress(_HeistEquipmentItem.Value.RequiredJobKey));
	}
}
