using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.FilesInMemory;

public class LabyrinthTrial : RemoteMemoryObject
{
	public WorldArea area;

	private int id = -1;

	public int Id
	{
		get
		{
			if (id == -1)
			{
				return id = base.M.Read<int>(base.Address + 16);
			}
			return id;
		}
	}

	public WorldArea Area
	{
		get
		{
			if (area == null)
			{
				long address = base.M.Read<long>(base.Address + 8);
				area = base.TheGame.Files.WorldAreas.GetByAddress(address);
			}
			return area;
		}
	}
}
