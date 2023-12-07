using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace ExileCore.Shared;

public class PluginWrapper
{
	private readonly PluginAssemblyLoadContext _loadContext;

	private string _name;

	private readonly Lazy<FileSystemWatcher> _fileSystemWatcher;

	[Obsolete]
	public DateTime LastWrite { get; set; } = DateTime.MinValue;


	public double InitialiseTime { get; private set; }

	public bool Force => Plugin.Force;

	public string Name => _name = Plugin?.Name ?? _name ?? PathOnDisk;

	public int Order => Plugin.Order;

	public IPlugin Plugin { get; private set; }

	public string PathOnDisk { get; }

	public PluginKind Kind { get; }

	public bool CanRender { get; set; }

	public bool CanBeMultiThreading => Plugin.CanUseMultiThreading;

	public DebugInformation TickDebugInformation { get; }

	public DebugInformation RenderDebugInformation { get; }

	public bool IsEnable
	{
		get
		{
			try
			{
				ToggleNode toggleNode = Plugin?._Settings.Enable;
				return toggleNode != null && (bool)toggleNode;
			}
			catch (Exception e)
			{
				LogError(e, "IsEnable");
				return false;
			}
		}
	}

	[Obsolete]
	public PluginWrapper(IPlugin plugin, string pathOnDisk)
		: this(plugin, pathOnDisk, null, PluginKind.Unknown)
	{
	}

	internal PluginWrapper(IPlugin plugin, string pathOnDisk, PluginAssemblyLoadContext loadContext, PluginKind kind)
	{
		_loadContext = loadContext;
		Plugin = plugin;
		_name = Plugin.Name;
		PathOnDisk = pathOnDisk;
		Kind = kind;
		_fileSystemWatcher = new Lazy<FileSystemWatcher>(() => new FileSystemWatcher
		{
			NotifyFilter = (NotifyFilters.LastWrite | NotifyFilters.CreationTime),
			Path = Plugin.DirectoryFullName,
			EnableRaisingEvents = true
		});
		TickDebugInformation = new DebugInformation(Name + " [P]", "plugin");
		RenderDebugInformation = new DebugInformation(Name + " [R]", "plugin");
	}

	public void CorrectThisTick(float val)
	{
		TickDebugInformation.CorrectAfterTick(val);
	}

	public void Onload()
	{
		try
		{
			Plugin.OnLoad();
		}
		catch (Exception e)
		{
			LogError(e, "Onload");
		}
	}

	public void Initialise(GameController _gameController)
	{
		try
		{
			if (Plugin._Settings == null)
			{
				throw new NullReferenceException("Cant load plugin (" + Plugin.Name + ") because settings is null.");
			}
			if (Plugin.Initialized)
			{
				throw new InvalidOperationException("Already initialized.");
			}
			Plugin._Settings.Enable.OnValueChanged += delegate(object? obj, bool value)
			{
				try
				{
					if (Plugin.Initialized)
					{
						List<Coroutine> list = (from x in Core.MainRunner.Coroutines.Concat(Core.ParallelRunner.Coroutines)
							where x.Owner == Plugin
							select x).ToList();
						if (value)
						{
							foreach (Coroutine item in list)
							{
								item.Resume();
							}
						}
						else
						{
							foreach (Coroutine item2 in list)
							{
								item2.Pause();
							}
						}
					}
					if (value && !Plugin.Initialized)
					{
						Plugin.Initialized = pluginInitialise();
						if (Plugin.Initialized)
						{
							Plugin.AreaChange(_gameController.Area.CurrentArea);
						}
					}
					if (value && !Plugin.Initialized)
					{
						Plugin._Settings.Enable.Value = false;
					}
				}
				catch (Exception e2)
				{
					LogError(e2, "Initialise");
				}
			};
			if ((bool)Plugin._Settings.Enable)
			{
				if (Plugin.Initialized)
				{
					throw new InvalidOperationException("Already initialized.");
				}
				Plugin.Initialized = pluginInitialise();
				if (!Plugin.Initialized)
				{
					Plugin._Settings.Enable.Value = false;
				}
			}
		}
		catch (Exception e)
		{
			LogError(e, "Initialise");
		}
	}

	private bool pluginInitialise()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		bool num = Plugin.Initialise();
		stopwatch.Stop();
		if (num)
		{
			double value = (InitialiseTime = stopwatch.Elapsed.TotalMilliseconds);
			DebugWindow.LogMsg($"{Name} -> Initialise time: {value} ms.", 1f, Color.Yellow);
		}
		return num;
	}

	public void SubscrideOnFile(Action<PluginWrapper, FileSystemEventArgs> action)
	{
		_fileSystemWatcher.Value.Changed += delegate(object _, FileSystemEventArgs args)
		{
			action?.Invoke(this, args);
		};
	}

	public void TurnOnOffPlugin(bool state)
	{
		Plugin._Settings.Enable.Value = state;
	}

	public void AreaChange(AreaInstance area)
	{
		try
		{
			Plugin.AreaChange(area);
		}
		catch (Exception e)
		{
			LogError(e, "AreaChange");
		}
	}

	public Job PerfomanceTick()
	{
		try
		{
			using (TickDebugInformation.Measure())
			{
				return Plugin.Tick();
			}
		}
		catch (Exception e)
		{
			LogError(e, "PerfomanceTick");
			return null;
		}
	}

	public Job Tick()
	{
		try
		{
			return Plugin.Tick();
		}
		catch (Exception e)
		{
			LogError(e, "Tick");
			return null;
		}
	}

	public void PerfomanceRender()
	{
		try
		{
			using (RenderDebugInformation.Measure())
			{
				Plugin.Render();
			}
		}
		catch (Exception e)
		{
			LogError(e, "PerfomanceRender");
		}
	}

	public void Render()
	{
		try
		{
			Plugin.Render();
		}
		catch (Exception e)
		{
			LogError(e, "Render");
		}
	}

	private void LogError(Exception e, [CallerMemberName] string methodName = null)
	{
		DebugWindow.LogError($"{Plugin?.Name ?? PathOnDisk}, {methodName} -> {e}", 3f);
	}

	public void EntityIgnored(Entity entity)
	{
		try
		{
			Plugin.EntityIgnored(entity);
		}
		catch (Exception e)
		{
			LogError(e, "EntityIgnored");
		}
	}

	public void EntityAddedAny(Entity entity)
	{
		try
		{
			Plugin.EntityAddedAny(entity);
		}
		catch (Exception e)
		{
			LogError(e, "EntityAddedAny");
		}
	}

	public void EntityAdded(Entity entity)
	{
		try
		{
			Plugin.EntityAdded(entity);
		}
		catch (Exception e)
		{
			LogError(e, "EntityAdded");
		}
	}

	public void EntityRemoved(Entity entity)
	{
		try
		{
			Plugin.EntityRemoved(entity);
		}
		catch (Exception e)
		{
			LogError(e, "EntityRemoved");
		}
	}

	public void ReceiveEvent(string eventId, object args)
	{
		try
		{
			Plugin.ReceiveEvent(eventId, args);
		}
		catch (Exception e)
		{
			LogError(e, "ReceiveEvent");
		}
	}

	public void SetApi(GameController gameController, Graphics graphics, PluginManager pluginManager)
	{
		Plugin.SetApi(gameController, graphics, pluginManager);
	}

	public void LoadSettings()
	{
		Plugin._LoadSettings();
	}

	public void Close()
	{
		try
		{
			if (_fileSystemWatcher.IsValueCreated)
			{
				_fileSystemWatcher.Value.Dispose();
			}
			Plugin._SaveSettings();
			Plugin.OnPluginDestroyForHotReload();
			Plugin.OnClose();
			foreach (Coroutine item in (from x in Core.MainRunner.Coroutines.Concat(Core.ParallelRunner.Coroutines)
				where x.Owner == Plugin
				select x).ToList())
			{
				try
				{
					item.Done(force: true);
				}
				catch (Exception e)
				{
					LogError(e, "Close");
				}
			}
			Plugin.OnUnload();
			Plugin.Dispose();
			Plugin = null;
			_loadContext.UnloadOnce();
		}
		catch (Exception e2)
		{
			LogError(e2, "Close");
		}
	}

	public void DrawSettings()
	{
		Plugin.DrawSettings();
	}

	public override string ToString()
	{
		return $"{Name} [{Order}]";
	}
}
