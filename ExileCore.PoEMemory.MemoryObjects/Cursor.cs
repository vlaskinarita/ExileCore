using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class Cursor : Element
{
	private readonly CachedValue<CursorOffsets> _cachevalue;

	public MouseActionType Action => (MouseActionType)base.M.Read<int>(base.Address + 896);

	public MouseActionType ActionCached => (MouseActionType)_cachevalue.Value.Action;

	public int ClicksCached => _cachevalue.Value.Clicks;

	public int Clicks => base.M.Read<int>(base.Address + 588);

	public string ActionString => base.M.ReadNativeString(base.Address + 672);

	public string ActionStringCached => _cachevalue.Value.ActionString.ToString(base.M);

	public Cursor()
	{
		_cachevalue = new FrameCache<CursorOffsets>(() => base.M.Read<CursorOffsets>(base.Address));
	}
}
