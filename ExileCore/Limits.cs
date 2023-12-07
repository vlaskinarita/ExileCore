using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ExileCore;

internal static class Limits
{
	private class LimitsInstance
	{
		public int? ElementChildCount;

		public int? UnicodeStringLength;

		public int? ReadStructsArrayCount;

		public int? ReadMemoryTimeLimit;
	}

	public static readonly int ElementChildCount;

	public static readonly int UnicodeStringLength;

	public static readonly int ReadStructsArrayCount;

	public static readonly int ReadMemoryTimeLimit;

	static Limits()
	{
		ElementChildCount = 1000;
		UnicodeStringLength = 5120;
		ReadStructsArrayCount = 100000;
		ReadMemoryTimeLimit = 2000;
		try
		{
			string path = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config", "limits.json");
			if (File.Exists(path))
			{
				LimitsInstance? limitsInstance = JsonConvert.DeserializeObject<LimitsInstance>(File.ReadAllText(path));
				int? elementChildCount = limitsInstance.ElementChildCount;
				if (elementChildCount.HasValue)
				{
					int valueOrDefault = elementChildCount.GetValueOrDefault();
					ElementChildCount = valueOrDefault;
				}
				elementChildCount = limitsInstance.UnicodeStringLength;
				if (elementChildCount.HasValue)
				{
					int valueOrDefault2 = elementChildCount.GetValueOrDefault();
					UnicodeStringLength = valueOrDefault2;
				}
				elementChildCount = limitsInstance.ReadStructsArrayCount;
				if (elementChildCount.HasValue)
				{
					int valueOrDefault3 = elementChildCount.GetValueOrDefault();
					ReadStructsArrayCount = valueOrDefault3;
				}
				elementChildCount = limitsInstance.ReadMemoryTimeLimit;
				if (elementChildCount.HasValue)
				{
					int valueOrDefault4 = elementChildCount.GetValueOrDefault();
					ReadMemoryTimeLimit = valueOrDefault4;
				}
			}
		}
		catch (Exception value)
		{
			try
			{
				string text = $"Unable to load the limits file: {value}";
				Logger.Log.Error(text);
				DebugWindow.LogError(text);
			}
			catch
			{
			}
		}
	}
}
