using System.Collections.Generic;

namespace ExileCore.PoEMemory.Elements.AtlasElements;

public class AtlasMasterMissionPanelElement : Element
{
	public Element AtlasMasterMissionInfoIcon => GetChildAtIndex(0);

	public Element AtlasMasterMissions => GetChildFromIndices(1, 0);

	public Dictionary<MasterMissionColour, int> EinharMissions
	{
		get
		{
			Dictionary<MasterMissionColour, int> dictionary = new Dictionary<MasterMissionColour, int>();
			dictionary.Add(MasterMissionColour.White, int.Parse(AtlasMasterMissions.GetChildFromIndices(1, 0, 0, 0).Text));
			dictionary.Add(MasterMissionColour.Yellow, int.Parse(AtlasMasterMissions.GetChildFromIndices(1, 0, 1, 0).Text));
			dictionary.Add(MasterMissionColour.Red, int.Parse(AtlasMasterMissions.GetChildFromIndices(1, 0, 2, 0).Text));
			return dictionary;
		}
	}

	public Dictionary<MasterMissionColour, int> AlvaMissions
	{
		get
		{
			Dictionary<MasterMissionColour, int> dictionary = new Dictionary<MasterMissionColour, int>();
			dictionary.Add(MasterMissionColour.White, int.Parse(AtlasMasterMissions.GetChildFromIndices(2, 0, 0, 0).Text));
			dictionary.Add(MasterMissionColour.Yellow, int.Parse(AtlasMasterMissions.GetChildFromIndices(2, 0, 1, 0).Text));
			dictionary.Add(MasterMissionColour.Red, int.Parse(AtlasMasterMissions.GetChildFromIndices(2, 0, 2, 0).Text));
			return dictionary;
		}
	}

	public Dictionary<MasterMissionColour, int> NikoMissions
	{
		get
		{
			Dictionary<MasterMissionColour, int> dictionary = new Dictionary<MasterMissionColour, int>();
			dictionary.Add(MasterMissionColour.White, int.Parse(AtlasMasterMissions.GetChildFromIndices(4, 0, 0, 0).Text));
			dictionary.Add(MasterMissionColour.Yellow, int.Parse(AtlasMasterMissions.GetChildFromIndices(4, 0, 1, 0).Text));
			dictionary.Add(MasterMissionColour.Red, int.Parse(AtlasMasterMissions.GetChildFromIndices(4, 0, 2, 0).Text));
			return dictionary;
		}
	}

	public Dictionary<MasterMissionColour, int> JunMissions
	{
		get
		{
			Dictionary<MasterMissionColour, int> dictionary = new Dictionary<MasterMissionColour, int>();
			dictionary.Add(MasterMissionColour.White, int.Parse(AtlasMasterMissions.GetChildFromIndices(5, 0, 0, 0).Text));
			dictionary.Add(MasterMissionColour.Yellow, int.Parse(AtlasMasterMissions.GetChildFromIndices(5, 0, 1, 0).Text));
			dictionary.Add(MasterMissionColour.Red, int.Parse(AtlasMasterMissions.GetChildFromIndices(5, 0, 2, 0).Text));
			return dictionary;
		}
	}

	public Dictionary<MasterMissionColour, int> KiracMissions
	{
		get
		{
			Dictionary<MasterMissionColour, int> dictionary = new Dictionary<MasterMissionColour, int>();
			dictionary.Add(MasterMissionColour.White, int.Parse(AtlasMasterMissions.GetChildFromIndices(6, 0, 0, 0).Text));
			dictionary.Add(MasterMissionColour.Yellow, int.Parse(AtlasMasterMissions.GetChildFromIndices(6, 0, 1, 0).Text));
			dictionary.Add(MasterMissionColour.Red, int.Parse(AtlasMasterMissions.GetChildFromIndices(6, 0, 2, 0).Text));
			return dictionary;
		}
	}
}
