using System;

namespace ExileCore.Shared.Cache;

public class CacheUtils
{
	public static Func<T> RememberLastValue<T>(Func<T, T> valueProducer, T initialValue = default(T))
	{
		return () => initialValue = valueProducer(initialValue);
	}
}
