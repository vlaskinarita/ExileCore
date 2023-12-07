using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using GameOffsets;

namespace ExileCore.PoEMemory.FilesInMemory.Labyrinth;

public class LabyrinthArea : RemoteMemoryObject
{
	private string _id;

	private List<WorldArea> _normalWorldAreas;

	private List<WorldArea> _cruelWorldAreas;

	private List<WorldArea> _mercilessWorldAreas;

	private List<WorldArea> _endgameWorldAreas;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public List<WorldArea> NormalWorldAreas => _normalWorldAreas ?? (_normalWorldAreas = GetAreas(8));

	public List<WorldArea> CruelWorldAreas => _cruelWorldAreas ?? (_cruelWorldAreas = GetAreas(24));

	public List<WorldArea> MercilessWorldAreas => _mercilessWorldAreas ?? (_mercilessWorldAreas = GetAreas(40));

	public List<WorldArea> EndgameWorldAreas => _endgameWorldAreas ?? (_endgameWorldAreas = GetAreas(56));

	private List<WorldArea> GetAreas(int offset)
	{
		return base.M.Read<DatArrayStruct>(base.Address + offset).ReadDatPtr(base.M).Select(base.TheGame.Files.WorldAreas.GetByAddress)
			.ToList();
	}

	public override string ToString()
	{
		return $"{Id} {base.Address:X}";
	}
}
