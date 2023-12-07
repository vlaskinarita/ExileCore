using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects;

public class WorldArea : RemoteMemoryObject
{
	private List<WorldArea> connections;

	private List<WorldArea> corruptedAreas;

	private string id;

	private string name;

	public string Id
	{
		get
		{
			if (id == null)
			{
				return id = base.M.ReadStringU(base.M.Read<long>(base.Address));
			}
			return id;
		}
	}

	public int Index { get; set; }

	public string Name
	{
		get
		{
			if (name == null)
			{
				return name = base.M.ReadStringU(base.M.Read<long>(base.Address + 8), 255);
			}
			return name;
		}
	}

	public int Act => base.M.Read<int>(base.Address + 16);

	public bool IsTown => base.M.Read<byte>(base.Address + 20) == 1;

	public bool IsHideout
	{
		get
		{
			if (Name.Contains("Hideout"))
			{
				return !Name.Contains("Syndicate Hideout");
			}
			return false;
		}
	}

	public bool HasWaypoint => base.M.Read<byte>(base.Address + 21) == 1;

	public int AreaLevel => base.M.Read<int>(base.Address + 38);

	public int WorldAreaId => base.M.Read<int>(base.Address + 42);

	public bool IsAtlasMap => Id.StartsWith("MapAtlas");

	public bool IsMapWorlds => Id.StartsWith("MapWorlds");

	public bool IsCorruptedArea
	{
		get
		{
			if (!Id.Contains("SideArea"))
			{
				return Id.Contains("Sidearea");
			}
			return true;
		}
	}

	public bool IsMissionArea => Id.Contains("Mission");

	public bool IsDailyArea => Id.Contains("Daily");

	public bool IsMapTrialArea => Id.StartsWith("EndGame_Labyrinth_trials");

	public bool IsLabyrinthArea
	{
		get
		{
			if (!IsMapTrialArea)
			{
				return Id.Contains("Labyrinth");
			}
			return false;
		}
	}

	public bool IsAbyssArea
	{
		get
		{
			if (!Id.Equals("AbyssLeague") && !Id.Equals("AbyssLeague2") && !Id.Equals("AbyssLeagueBoss") && !Id.Equals("AbyssLeagueBoss2"))
			{
				return Id.Equals("AbyssLeagueBoss3");
			}
			return true;
		}
	}

	public bool IsUnique => base.M.Read<bool>(base.Address + 492);

	public IList<WorldArea> Connections
	{
		get
		{
			if (connections == null)
			{
				connections = new List<WorldArea>();
				int num = base.M.Read<int>(base.Address + 22);
				long num2 = base.M.Read<long>(base.Address + 30);
				if (num > 30)
				{
					return connections;
				}
				for (int i = 0; i < num; i++)
				{
					WorldArea byAddress = base.TheGame.Files.WorldAreas.GetByAddress(base.M.Read<long>(num2));
					connections.Add(byAddress);
					num2 += 8;
				}
			}
			return connections;
		}
	}

	public IList<WorldArea> CorruptedAreas
	{
		get
		{
			if (corruptedAreas == null)
			{
				corruptedAreas = new List<WorldArea>();
				long num = base.M.Read<long>(base.Address + 259);
				int num2 = base.M.Read<int>(base.Address + 251);
				if (num2 > 30)
				{
					return corruptedAreas;
				}
				for (int i = 0; i < num2; i++)
				{
					WorldArea byAddress = base.TheGame.Files.WorldAreas.GetByAddress(base.M.Read<long>(num));
					corruptedAreas.Add(byAddress);
					num += 8;
				}
			}
			return corruptedAreas;
		}
	}

	public override string ToString()
	{
		return Name ?? "";
	}
}
