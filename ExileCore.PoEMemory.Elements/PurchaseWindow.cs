using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Elements;

public class PurchaseWindow : Element
{
	private readonly CachedValue<PurchaseWindowOffsets> _cache;

	public Element CloseButton => base[2];

	public VendorStashTabContainer TabContainer => GetObject<VendorStashTabContainer>(_cache.Value.StashTabContainerPtr);

	public PurchaseWindow()
	{
		_cache = CreateStructFrameCache<PurchaseWindowOffsets>();
	}
}
