using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class MinimapIcon : Component
{
	private FrameCache<MinimapIconOffsets> cachedValue;

	private MinimapIconOffsets MinimapIconOffsets => cachedValue.Value;

	public bool IsVisible => MinimapIconOffsets.IsVisible == 0;

	public bool IsHide => MinimapIconOffsets.IsHide == 1;

	public string TestString => base.M.ReadStringU(base.M.Read<long>(MinimapIconOffsets.NamePtr));

	public string Name => RemoteMemoryObject.Cache.StringCache.Read($"{MinimapIconOffsets.NamePtr}{base.Address}", () => TestString);

	public MinimapIcon()
	{
		cachedValue = new FrameCache<MinimapIconOffsets>(() => base.M.Read<MinimapIconOffsets>(base.Address));
	}
}
