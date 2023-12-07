using ExileCore.Shared.Helpers;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class RenderItem : Component
{
	private const int ResourcePathOffset = 40;

	public string ResourcePath
	{
		get
		{
			NativeUtf16Text text = base.M.Read<NativeUtf16Text>(base.Address + 40);
			return RemoteMemoryObject.Cache.StringCache.Read("RenderItem" + text.CacheString, () => text.ToString(base.M));
		}
	}
}
