using System;

namespace ExileCore.Shared.Cache;

public class AreaCache<T> : CachedValue<T>
{
	private uint _areaHash;

	private uint _forceRefreshCounter;

	public AreaCache(Func<T> func)
		: base(func)
	{
		_areaHash = uint.MaxValue;
	}

	protected override bool Update(bool force)
	{
		if (_areaHash != AreaInstance.CurrentHash || _forceRefreshCounter != AreaInstance.ForceRefreshCounter || force)
		{
			_areaHash = AreaInstance.CurrentHash;
			_forceRefreshCounter = AreaInstance.ForceRefreshCounter;
			return true;
		}
		return false;
	}
}
