using System;
using System.Diagnostics;
using System.Threading;

namespace ExileCore.Shared.Cache;

public abstract class CachedValue
{
	public static int TotalCount;

	public static int LifeCount;

	public static float Latency { get; set; } = 25f;

}
public abstract class CachedValue<T> : CachedValue
{
	public delegate void CacheUpdateEvent(T t);

	protected static Stopwatch sw = Stopwatch.StartNew();

	private readonly Func<T> _func;

	private bool _force;

	private T _value;

	private bool _updated;

	public T Value
	{
		get
		{
			if (Update(_force))
			{
				_force = false;
				_value = _func();
				this.OnUpdate?.Invoke(_value);
				_updated = true;
				return _value;
			}
			if (!_updated)
			{
				return _func();
			}
			return _value;
		}
	}

	public T RealValue => _func();

	public event CacheUpdateEvent OnUpdate;

	protected CachedValue(Func<T> func)
	{
		_func = func ?? throw new ArgumentNullException("func", "Cached Value ctor null function");
		Interlocked.Increment(ref CachedValue.TotalCount);
		Interlocked.Increment(ref CachedValue.LifeCount);
	}

	public void ForceUpdate()
	{
		_force = true;
	}

	protected abstract bool Update(bool force);

	~CachedValue()
	{
		Interlocked.Decrement(ref CachedValue.LifeCount);
	}
}
