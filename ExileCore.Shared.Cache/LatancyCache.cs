using System;

namespace ExileCore.Shared.Cache;

public class LatancyCache<T> : CachedValue<T>
{
	private readonly int _minLatency;

	private long _checkTime;

	public LatancyCache(Func<T> func, int minLatency = 10)
		: base(func)
	{
		_minLatency = minLatency;
		_checkTime = long.MinValue;
	}

	protected override bool Update(bool force)
	{
		float latency = CachedValue.Latency;
		long elapsedMilliseconds = CachedValue<T>.sw.ElapsedMilliseconds;
		if (elapsedMilliseconds >= _checkTime || force)
		{
			if (latency > (float)_minLatency)
			{
				_checkTime = (long)((float)elapsedMilliseconds + latency);
			}
			else
			{
				_checkTime = elapsedMilliseconds + _minLatency;
			}
			return true;
		}
		return false;
	}
}
