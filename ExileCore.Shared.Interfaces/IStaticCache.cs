using System;

namespace ExileCore.Shared.Interfaces;

public interface IStaticCache
{
	int Count { get; }

	int DeletedCache { get; }

	int ReadCache { get; }

	int ReadMemory { get; }

	string CoeffString { get; }

	float Coeff { get; }

	void UpdateCache();

	bool Remove(string key);
}
public interface IStaticCache<T> : IStaticCache
{
	T Read(string addr, Func<T> func);
}
