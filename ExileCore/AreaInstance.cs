using System;
using ExileCore.PoEMemory.MemoryObjects;
using SharpDX;

namespace ExileCore;

public sealed class AreaInstance
{
	public static uint CurrentHash;

	public static uint ForceRefreshCounter;

	public int RealLevel { get; }

	public string Name { get; }

	public int Act { get; }

	public bool IsTown { get; }

	public bool IsHideout { get; }

	public bool HasWaypoint { get; }

	public uint Hash { get; }

	public AreaTemplate Area { get; }

	public string DisplayName => Name + " (" + RealLevel + ")";

	public DateTime TimeEntered { get; } = DateTime.UtcNow;


	public Color AreaColorName { get; set; } = Color.Aqua;


	public AreaInstance(AreaTemplate area, uint hash, int realLevel)
	{
		Area = area;
		Hash = hash;
		RealLevel = realLevel;
		Name = area.Name;
		Act = area.Act;
		IsTown = area.IsTown || Name.Equals("The Rogue Harbour");
		IsHideout = Name.Contains("Hideout") && !Name.Contains("Syndicate Hideout");
		HasWaypoint = area.HasWaypoint || IsHideout;
	}

	public override string ToString()
	{
		return $"{Name} ({RealLevel}) #{Hash}";
	}

	public static string GetTimeString(TimeSpan timeSpent)
	{
		int num = (int)timeSpent.TotalSeconds;
		int num2 = num % 60;
		int num3 = num / 60;
		int num4 = num3 / 60;
		num3 %= 60;
		return string.Format((num4 > 0) ? "{0}:{1:00}:{2:00}" : "{1}:{2:00}", num4, num3, num2);
	}
}
