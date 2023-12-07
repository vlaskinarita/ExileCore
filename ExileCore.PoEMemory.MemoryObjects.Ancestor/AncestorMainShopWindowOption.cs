using ExileCore.PoEMemory.FilesInMemory.Ancestor;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects.Ancestor;

public class AncestorMainShopWindowOption : Element
{
	private readonly CachedValue<AncestorShopWindowOffsets> _cachedValue;

	public AncestralTrialUnit Unit => base.TheGame.Files.AncestralTrialUnits.GetByAddress(_cachedValue.Value.UnitPtr);

	public AncestralTrialItem Item => base.TheGame.Files.AncestralTrialItems.GetByAddress(_cachedValue.Value.ItemPtr);

	public AncestorMainShopWindowOption()
	{
		_cachedValue = CreateStructFrameCache<AncestorShopWindowOffsets>();
	}
}
