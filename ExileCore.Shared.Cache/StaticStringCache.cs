using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SharpDX;

namespace ExileCore.Shared.Cache;

public class StaticStringCache
{
	private readonly ConcurrentDictionary<IntPtr, DateTime> _lastAccess = new ConcurrentDictionary<IntPtr, DateTime>();

	private readonly int _lifeTimeForCache;

	private DateTime lastClear;

	private readonly object locker = new object();

	public Dictionary<IntPtr, string> Debug { get; } = new Dictionary<IntPtr, string>();


	public int Count => Debug.Count;

	public StaticStringCache(int LifeTimeForCache = 300)
	{
		_lifeTimeForCache = LifeTimeForCache;
	}

	public int ClearByTime()
	{
		int num = 0;
		if ((DateTime.UtcNow - lastClear).TotalSeconds < 60.0)
		{
			return num;
		}
		foreach (KeyValuePair<IntPtr, DateTime> item in _lastAccess)
		{
			if ((DateTime.UtcNow - item.Value).TotalSeconds > (double)_lifeTimeForCache && Debug.Remove(item.Key))
			{
				num++;
				_lastAccess.TryRemove(item.Key, out var _);
			}
		}
		if (_lastAccess.Count > 30000)
		{
			_lastAccess.Clear();
			Debug.Clear();
			DebugWindow.LogMsg("Clear CACHE because so big (>30k)", 7f, Color.GreenYellow);
		}
		lastClear = DateTime.UtcNow;
		DebugWindow.LogMsg($"StaticStringCache Cleared by time: {num} [{lastClear}]", 7f, Color.Yellow);
		return num;
	}

	public string Read(IntPtr addr, Func<string> func)
	{
		if (Debug.TryGetValue(addr, out var value))
		{
			_lastAccess[addr] = DateTime.UtcNow;
			return value;
		}
		value = func();
		lock (locker)
		{
			Debug[addr] = value;
		}
		_lastAccess[addr] = DateTime.UtcNow;
		return value;
	}
}
