using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.AtlasHelper;
using ExileCore.Shared.Interfaces;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore;

public abstract class BaseSettingsPlugin<TSettings> : IPlugin, IDisposable where TSettings : ISettings, new()
{
	private const string TEXTURES_FOLDER = "textures";

	private AtlasTexturesProcessor _atlasTextures;

	private PluginManager _pluginManager;

	public List<ISettingsHolder> Drawers { get; }

	public GameController GameController { get; private set; }

	public Graphics Graphics { get; private set; }

	public TSettings Settings => (TSettings)_Settings;

	public ISettings _Settings { get; private set; }

	public bool CanUseMultiThreading { get; protected set; }

	public string Description { get; protected set; }

	public string DirectoryName { get; set; }

	public string DirectoryFullName { get; set; }

	public bool Force { get; protected set; }

	public bool Initialized { get; set; }

	public string InternalName { get; }

	public string Name { get; set; }

	public int Order { get; protected set; }

	public string ConfigDirectory => GameController.Settings.GetPluginSettingsDirectory(this);

	protected PluginManager PluginManager => _pluginManager;

	public CancellationToken ZoneCancellationToken => _pluginManager.ZoneCancellationToken;

	protected BaseSettingsPlugin()
	{
		InternalName = GetType().Namespace;
		if (string.IsNullOrWhiteSpace(Name))
		{
			Name = InternalName;
		}
		Drawers = new List<ISettingsHolder>();
	}

	public void _LoadSettings()
	{
		try
		{
			string text = GameController.Settings.LoadSettings(this);
			if (text != null)
			{
				_Settings = JsonConvert.DeserializeObject<TSettings>(text, SettingsContainer.jsonSettings);
			}
		}
		catch (Exception ex)
		{
			DebugWindow.LogError(ex.ToString());
		}
		if (_Settings == null)
		{
			ISettings settings2 = (_Settings = new TSettings());
		}
		SettingsParser.Parse(_Settings, Drawers);
	}

	public void _SaveSettings()
	{
		if (_Settings == null)
		{
			throw new NullReferenceException("Plugin settings is null");
		}
		GameController.Settings.SaveSettings(this);
	}

	public virtual void AreaChange(AreaInstance area)
	{
	}

	public virtual void Dispose()
	{
		OnClose();
	}

	public virtual void DrawSettings()
	{
		foreach (ISettingsHolder drawer in Drawers)
		{
			drawer.Draw();
		}
	}

	public virtual void EntityAdded(Entity entity)
	{
	}

	public virtual void EntityAddedAny(Entity entity)
	{
	}

	public virtual void EntityIgnored(Entity entity)
	{
	}

	public virtual void EntityRemoved(Entity entity)
	{
	}

	public virtual void OnLoad()
	{
	}

	public virtual void OnUnload()
	{
	}

	public virtual bool Initialise()
	{
		return true;
	}

	public void LogMsg(string msg)
	{
		DebugWindow.LogMsg(msg);
	}

	public virtual void OnClose()
	{
		_SaveSettings();
	}

	public virtual void ReceiveEvent(string eventId, object args)
	{
	}

	public void PublishEvent(string eventId, object args)
	{
		_pluginManager.ReceivePluginEvent(eventId, args, this);
	}

	public virtual void OnPluginSelectedInMenu()
	{
	}

	public virtual Job Tick()
	{
		return null;
	}

	public virtual void Render()
	{
	}

	public void LogError(string msg, float time = 1f)
	{
		DebugWindow.LogError(msg, time);
	}

	public void LogMessage(string msg, float time, Color clr)
	{
		DebugWindow.LogMsg(msg, time, clr);
	}

	public void LogMessage(string msg, float time = 1f)
	{
		DebugWindow.LogMsg(msg, time, Color.GreenYellow);
	}

	public virtual void OnPluginDestroyForHotReload()
	{
	}

	public void SetApi(GameController gameController, Graphics graphics, PluginManager pluginManager)
	{
		GameController = gameController;
		Graphics = graphics;
		_pluginManager = pluginManager;
	}

	public AtlasTexture GetAtlasTexture(string textureName)
	{
		if (_atlasTextures == null)
		{
			string text = Path.Combine(DirectoryFullName, "textures");
			string[] files = Directory.GetFiles(text, "*.json");
			if (files.Length == 0)
			{
				LogError($"Plugin '{Name}': Can't find atlas json config file in '{text}' " + "(expecting config 'from Free texture packer' program)", 20f);
				_atlasTextures = new AtlasTexturesProcessor("%AtlasNotFound%");
				return null;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[0]);
			if (files.Length > 1)
			{
				LogError($"Plugin '{Name}': Found multiple atlas configs in folder '{text}', selecting the first one ''{fileNameWithoutExtension}''", 20f);
			}
			string text2 = Path.Combine(DirectoryFullName, "textures\\" + fileNameWithoutExtension + ".png");
			if (!File.Exists(text2))
			{
				LogError($"Plugin '{Name}': Can't find atlas png texture file in '{text2}' ", 20f);
				_atlasTextures = new AtlasTexturesProcessor(fileNameWithoutExtension);
				return null;
			}
			_atlasTextures = new AtlasTexturesProcessor(files[0], text2);
			Graphics.InitImage(text2, textures: false);
		}
		return _atlasTextures.GetTextureByName(textureName);
	}

	public AtlasTexturesProcessor CreateAtlas(string configPath, string texturePath)
	{
		if (!File.Exists(configPath))
		{
			LogError($"Plugin '{Name}': Can't find atlas json config file in '{configPath}'", 20f);
			return new AtlasTexturesProcessor("%AtlasNotFound%");
		}
		if (!File.Exists(texturePath))
		{
			LogError($"Plugin '{Name}': Can't find atlas png texture file in '{texturePath}' ", 20f);
			return new AtlasTexturesProcessor("%AtlasNotFound%");
		}
		Graphics.InitImage(texturePath, textures: false);
		return new AtlasTexturesProcessor(configPath, texturePath);
	}
}
