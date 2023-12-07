using System.Collections.Generic;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ExpeditionAreaData : RemoteMemoryObject
{
	public const int StructSize = 192;

	private readonly CachedValue<ExpeditionAreaDataOffsets> _cachedValue;

	public ExpeditionAreaDataOffsets ExpeditionAreaDataStruct => _cachedValue.Value;

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address, new int[2] { 0, 8 }));

	public string Faction => base.M.ReadStringU(base.M.Read<long>(base.Address + 16, new int[1] { 8 }));

	public List<ItemMod> ImplicitMods => GetMods(ExpeditionAreaDataStruct.ModsData.First, ExpeditionAreaDataStruct.ModsData.Last);

	public ExpeditionAreaData()
	{
		_cachedValue = new FrameCache<ExpeditionAreaDataOffsets>(() => base.M.Read<ExpeditionAreaDataOffsets>(base.Address));
	}

	private List<ItemMod> GetMods(long startOffset, long endOffset)
	{
		List<ItemMod> list = new List<ItemMod>();
		if (base.Address == 0L)
		{
			return list;
		}
		if ((endOffset - startOffset) / ItemMod.STRUCT_SIZE > 12)
		{
			return list;
		}
		for (long num = startOffset; num < endOffset; num += ItemMod.STRUCT_SIZE)
		{
			list.Add(GetObject<ItemMod>(num));
		}
		return list;
	}
}
