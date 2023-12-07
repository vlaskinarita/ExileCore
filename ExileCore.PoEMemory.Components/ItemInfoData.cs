using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class ItemInfoData : RemoteMemoryObject
{
	private readonly CachedValue<ItemInfoOffsets> _CachedValue;

	public ItemInfoOffsets ItemInfoDataStruct => _CachedValue.Value;

	public byte ItemCellsSizeX => ItemInfoDataStruct.ItemCellsSizeX;

	public byte ItemCellsSizeY => ItemInfoDataStruct.ItemCellsSizeY;

	public string Name => ItemInfoDataStruct.Name.ToString(base.M);

	public string FlavourText => ItemInfoDataStruct.FlavourText.ToString(base.M);

	public long BaseItemType => ItemInfoDataStruct.BaseItemType;

	public NativePtrArray Tags => ItemInfoDataStruct.Tags;

	public ItemInfoData()
	{
		_CachedValue = new FrameCache<ItemInfoOffsets>(() => base.M.Read<ItemInfoOffsets>(base.Address));
	}
}
