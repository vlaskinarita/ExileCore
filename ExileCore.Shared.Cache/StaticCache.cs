using System;
using System.Runtime.Caching;
using System.Threading;
using ExileCore.Shared.Interfaces;

namespace ExileCore.Shared.Cache;

public class StaticCache<T> : IStaticCache<T>, IStaticCache
{
	private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

	private readonly int _lifeTimeForCache;

	private readonly string name;

	private readonly CacheItemPolicy _policy;

	private readonly MemoryCache cache;

	private bool IsEmpty = true;

	public int Count => ReadMemory - DeletedCache;

	public int DeletedCache { get; private set; }

	public int ReadCache { get; private set; }

	public int ReadMemory { get; private set; }

	public string CoeffString => $"{Coeff:0.000}% Read from memory";

	public float Coeff => (float)ReadMemory / (float)(ReadCache + ReadMemory) * 100f;

	public StaticCache(int lifeTimeForCache = 120, int limit = 30, string name = null)
	{
		_lifeTimeForCache = lifeTimeForCache;
		this.name = name ?? typeof(T).Name;
		cache = new MemoryCache(this.name);
		_policy = new CacheItemPolicy
		{
			SlidingExpiration = TimeSpan.FromSeconds(lifeTimeForCache),
			RemovedCallback = delegate
			{
				DeletedCache++;
			}
		};
	}

	public void UpdateCache()
	{
		if (!IsEmpty)
		{
			cache.Trim(100);
			IsEmpty = true;
		}
	}

	public T Read(string addr, Func<T> func)
	{
		cacheLock.EnterReadLock();
		try
		{
			IsEmpty = false;
			object obj = cache[addr];
			if (obj != null)
			{
				ReadCache++;
				return (T)obj;
			}
		}
		finally
		{
			cacheLock.ExitReadLock();
		}
		cacheLock.EnterUpgradeableReadLock();
		try
		{
			object obj2 = cache.Get(addr);
			if (obj2 != null)
			{
				ReadCache++;
				return (T)obj2;
			}
			try
			{
				cacheLock.EnterWriteLock();
				T val = func();
				ReadMemory++;
				cache.Add(addr, val, _policy);
				return val;
			}
			finally
			{
				cacheLock.ExitWriteLock();
			}
		}
		finally
		{
			cacheLock.ExitUpgradeableReadLock();
		}
	}

	public bool Remove(string key)
	{
		if (cache.Remove(key) != null)
		{
			return true;
		}
		return false;
	}
}
