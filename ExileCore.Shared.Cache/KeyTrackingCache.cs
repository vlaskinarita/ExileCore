using System;
using System.Collections.Generic;

namespace ExileCore.Shared.Cache;

public class KeyTrackingCache<T, TKey> : CachedValue<T>
{
	private readonly Func<TKey> _keyFunc;

	private TKey _lastKey;

	private bool _first;

	public KeyTrackingCache(Func<T> func, Func<TKey> keyFunc)
		: base(func)
	{
		_keyFunc = keyFunc;
		_first = true;
	}

	protected override bool Update(bool force)
	{
		TKey val = _keyFunc();
		bool result = _first || !EqualityComparer<TKey>.Default.Equals(val, _lastKey);
		_lastKey = val;
		_first = false;
		return result;
	}
}
public static class KeyTrackingCache
{
	public static KeyTrackingCache<T, TKey> Create<T, TKey>(Func<T> func, Func<TKey> keyFunc)
	{
		return new KeyTrackingCache<T, TKey>(func, keyFunc);
	}
}
