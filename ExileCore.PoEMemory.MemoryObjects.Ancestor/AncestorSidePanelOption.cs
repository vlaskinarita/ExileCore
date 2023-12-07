using ExileCore.PoEMemory.FilesInMemory.Ancestor;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects.Ancestor;

public class AncestorSidePanelOption : Element
{
	private readonly CachedValue<AncestorSidePanelOffsets> _cachedValue;

	public AncestralTrialUnit Unit => base.TheGame.Files.AncestralTrialUnits.GetByAddress(_cachedValue.Value.UnitPtr);

	public AncestralTrialItem Item => base.TheGame.Files.AncestralTrialItems.GetByAddress(_cachedValue.Value.ItemPtr);

	public AncestorSidePanelOption()
	{
		_cachedValue = CreateStructFrameCache<AncestorSidePanelOffsets>();
	}
}
