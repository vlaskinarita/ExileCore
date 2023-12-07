using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace ExileCore.Shared;

internal class PluginAssemblyLoadContext : AssemblyLoadContext
{
	private readonly string _pluginAssemblyLocation;

	private readonly bool _loadFromStream;

	private readonly AssemblyDependencyResolver _resolver;

	private int _unloadState;

	public PluginAssemblyLoadContext(string pluginAssemblyLocation, bool loadFromStream)
		: base(isCollectible: true)
	{
		_pluginAssemblyLocation = pluginAssemblyLocation;
		_loadFromStream = loadFromStream;
		_resolver = new AssemblyDependencyResolver(pluginAssemblyLocation);
		base.Resolving += ResolvingCallback;
		base.ResolvingUnmanagedDll += ResolvingUnmanagedDllCallback;
	}

	private Assembly ResolvingCallback(AssemblyLoadContext context, AssemblyName assemblyName)
	{
		string text = _resolver.ResolveAssemblyToPath(assemblyName);
		if (text == null)
		{
			string text2 = Path.Join(Path.GetDirectoryName(_pluginAssemblyLocation), assemblyName.Name + ".dll");
			if (File.Exists(text2))
			{
				text = text2;
			}
		}
		if (text != null)
		{
			if (_loadFromStream)
			{
				using (FileStream assembly = File.OpenRead(text))
				{
					string text3 = text;
					int length = ".exe".Length;
					string path = text3.Substring(0, text3.Length - length) + ".pdb";
					using FileStream assemblySymbols = (File.Exists(path) ? File.OpenRead(path) : null);
					return context.LoadFromStream(assembly, assemblySymbols);
				}
			}
			return context.LoadFromAssemblyPath(text);
		}
		return null;
	}

	private IntPtr ResolvingUnmanagedDllCallback(Assembly assembly, string dllName)
	{
		string text = _resolver.ResolveUnmanagedDllToPath(dllName);
		if (text != null)
		{
			return LoadUnmanagedDllFromPath(text);
		}
		return IntPtr.Zero;
	}

	public void UnloadOnce()
	{
		if (Interlocked.CompareExchange(ref _unloadState, 1, 0) == 0)
		{
			Unload();
		}
	}
}
