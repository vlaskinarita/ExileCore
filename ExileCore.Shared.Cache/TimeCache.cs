using System;

namespace ExileCore.Shared.Cache;

public class TimeCache<T> : CachedValue<T>
{
	private long _waitMilliseconds;

	private long time;

	public TimeCache(Func<T> func, long waitMilliseconds)
		: base(func)
	{
		time = long.MinValue;
		_waitMilliseconds = waitMilliseconds;
	}

	public void NewTime(long newTime)
	{
		_waitMilliseconds = newTime;
		time = _waitMilliseconds + CachedValue<T>.sw.ElapsedMilliseconds;
	}

	protected override bool Update(bool force)
	{
		long elapsedMilliseconds = CachedValue<T>.sw.ElapsedMilliseconds;
		if (elapsedMilliseconds >= time || force)
		{
			time = elapsedMilliseconds + _waitMilliseconds;
			return true;
		}
		return false;
	}
}
