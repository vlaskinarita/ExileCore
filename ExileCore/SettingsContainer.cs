using System;
using System.IO;
using System.Threading;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using Newtonsoft.Json;

namespace ExileCore;

public class SettingsContainer
{
	private const string SETTINGS_FILE_NAME = "settings.json";

	private const string DEFAULT_PROFILE_NAME = "global";

	private const string CFG_DIR_NAME = "config";

	private static readonly string CfgDirectoryPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "config");

	private static readonly string SettingsFilePath = Path.Join(CfgDirectoryPath, "settings.json");

	public static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
	{
		ContractResolver = new SortContractResolver(),
		Converters = new JsonConverter[3]
		{
			new ColorNodeConverter(),
			new ToggleNodeConverter(),
			new FileNodeConverter()
		}
	};

	private string _currentProfileName = "";

	public CoreSettings CoreSettings;

	private static ReaderWriterLockSlim rwLock { get; } = new ReaderWriterLockSlim();


	private string CurrentProfileName
	{
		get
		{
			return _currentProfileName;
		}
		set
		{
			this.OnProfileChange?.Invoke(this, value);
			_currentProfileName = value;
		}
	}

	public event EventHandler<string> OnProfileChange;

	public SettingsContainer()
	{
		Directory.CreateDirectory(Path.Join(CfgDirectoryPath, "global"));
		LoadCoreSettings();
	}

	internal static void ResetContractResolver()
	{
		jsonSettings.ContractResolver = new SortContractResolver();
	}

	public void LoadCoreSettings()
	{
		if (File.Exists(SettingsFilePath))
		{
			try
			{
				string value = File.ReadAllText(SettingsFilePath);
				CoreSettings = JsonConvert.DeserializeObject<CoreSettings>(value);
				CurrentProfileName = CoreSettings.Profiles.Value;
				return;
			}
			catch (Exception ex)
			{
				DebugWindow.LogError(ex.ToString());
			}
		}
		CoreSettings coreSettings = new CoreSettings();
		File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(coreSettings, Formatting.Indented));
		CoreSettings = coreSettings;
		CurrentProfileName = CoreSettings.Profiles.Value;
	}

	public void SaveCoreSettings()
	{
		rwLock.EnterWriteLock();
		try
		{
			string contents = JsonConvert.SerializeObject(CoreSettings, Formatting.Indented);
			if (new FileInfo(SettingsFilePath).Length > 1)
			{
				File.Copy(SettingsFilePath, Path.Join(CfgDirectoryPath, "dumpSettings.json"), overwrite: true);
			}
			File.WriteAllText(SettingsFilePath, contents);
		}
		catch (Exception ex)
		{
			DebugWindow.LogError(ex.ToString());
		}
		finally
		{
			rwLock.ExitWriteLock();
		}
	}

	public void SaveSettings(IPlugin plugin)
	{
		if (plugin == null)
		{
			return;
		}
		if (string.IsNullOrWhiteSpace(CurrentProfileName))
		{
			CurrentProfileName = "global";
		}
		rwLock.EnterWriteLock();
		try
		{
			Directory.CreateDirectory(Path.Join(CfgDirectoryPath, CurrentProfileName));
			File.WriteAllText(Path.Join(CfgDirectoryPath, CurrentProfileName, plugin.InternalName + "_settings.json"), JsonConvert.SerializeObject(plugin._Settings, Formatting.Indented, jsonSettings));
		}
		finally
		{
			rwLock.ExitWriteLock();
		}
	}

	public string LoadSettings(IPlugin plugin)
	{
		if (!Directory.Exists(Path.Join(CfgDirectoryPath, CurrentProfileName)))
		{
			throw new DirectoryNotFoundException(CurrentProfileName + " not found in " + CfgDirectoryPath);
		}
		string path = Path.Join(CfgDirectoryPath, CurrentProfileName, plugin.InternalName + "_settings.json");
		if (!File.Exists(path))
		{
			return null;
		}
		string text = File.ReadAllText(path);
		if (text.Length != 0)
		{
			return text;
		}
		return null;
	}

	public string GetPluginSettingsDirectory(IPlugin plugin)
	{
		return Directory.CreateDirectory(Path.Join(CfgDirectoryPath, plugin.InternalName)).FullName;
	}

	public static TSettingType LoadSettingFile<TSettingType>(string fileName)
	{
		if (!File.Exists(fileName))
		{
			Logger.Log.Error("Cannot find file '" + fileName + "'.");
			return default(TSettingType);
		}
		return JsonConvert.DeserializeObject<TSettingType>(File.ReadAllText(fileName));
	}

	public static void SaveSettingFile<TSettingType>(string fileName, TSettingType setting)
	{
		string contents = JsonConvert.SerializeObject(setting, Formatting.Indented);
		File.WriteAllText(fileName, contents);
	}
}
