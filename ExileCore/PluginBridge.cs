using System.Collections.Generic;

namespace ExileCore;

public class PluginBridge
{
	private readonly Dictionary<string, object> _methods = new Dictionary<string, object>();

	public T GetMethod<T>(string name) where T : class
	{
		if (_methods.TryGetValue(name, out var value))
		{
			return value as T;
		}
		return null;
	}

	public void SaveMethod(string name, object method)
	{
		_methods[name] = method;
	}
}
