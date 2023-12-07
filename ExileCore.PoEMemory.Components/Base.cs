using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class Base : Component
{
	private readonly CachedValue<BaseComponentOffsets> _cachedValue;

	private readonly CachedValue<ItemInfoData> _ItemInfoData;

	private string _name;

	public BaseComponentOffsets BaseStruct => _cachedValue.Value;

	public string Name => _name ?? (_name = _ItemInfoData.Value.Name);

	public byte ItemCellsSizeX => _ItemInfoData.Value.ItemCellsSizeX;

	public byte ItemCellsSizeY => _ItemInfoData.Value.ItemCellsSizeY;

	public Influence InfluenceFlag => (Influence)_cachedValue.Value.Influence;

	public bool isShaper => (InfluenceFlag & Influence.Shaper) == Influence.Shaper;

	public bool isElder => (InfluenceFlag & Influence.Elder) == Influence.Elder;

	public bool isCrusader => (InfluenceFlag & Influence.Crusader) == Influence.Crusader;

	public bool isHunter => (InfluenceFlag & Influence.Hunter) == Influence.Hunter;

	public bool isRedeemer => (InfluenceFlag & Influence.Redeemer) == Influence.Redeemer;

	public bool isWarlord => (InfluenceFlag & Influence.Warlord) == Influence.Warlord;

	public bool isSynthesized => base.M.Read<byte>(base.Address + 222) == 1;

	public bool isCorrupted => (_cachedValue.Value.Corrupted & 1) == 1;

	public int UnspentAbsorbedCorruption => _cachedValue.Value.UnspentAbsorbedCorruption;

	public int ScourgedTier => _cachedValue.Value.ScourgedTier;

	public string PublicPrice => _cachedValue.Value.PublicPrice.ToString(base.M);

	public Base()
	{
		_cachedValue = new FrameCache<BaseComponentOffsets>(() => base.M.Read<BaseComponentOffsets>(base.Address));
		_ItemInfoData = new FrameCache<ItemInfoData>(() => GetObject<ItemInfoData>(_cachedValue.Value.ItemInfo));
	}
}
