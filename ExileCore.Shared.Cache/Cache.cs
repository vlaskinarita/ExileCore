using ExileCore.Shared.Interfaces;

namespace ExileCore.Shared.Cache;

public class Cache
{
	public IStaticCache<string> StringCache { get; private set; }

	public Cache()
	{
		CreateCache();
	}

	public void CreateCache()
	{
		StringCache = new StaticCache<string>(300);
	}

	public void TryClearCache()
	{
		StringCache.UpdateCache();
	}
}
