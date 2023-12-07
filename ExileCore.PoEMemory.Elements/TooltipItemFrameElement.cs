using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Elements;

public class TooltipItemFrameElement : Element
{
	private readonly CachedValue<TooltipItemFrameElementOffsets> _cachedValue;

	public string CopyText => base.M.ReadStringU(_cachedValue.Value.CopyTextPtr, 5000);

	public bool IsAdvancedTooltipText => _cachedValue.Value.IsAdvancedTooltipText;

	public TooltipItemFrameElement()
	{
		_cachedValue = CreateStructFrameCache<TooltipItemFrameElementOffsets>();
	}
}
