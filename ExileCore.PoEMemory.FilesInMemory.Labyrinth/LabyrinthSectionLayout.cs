using System.Collections.Generic;
using System.Linq;
using GameOffsets;

namespace ExileCore.PoEMemory.FilesInMemory.Labyrinth;

public class LabyrinthSectionLayout : RemoteMemoryObject
{
	private LabyrinthSectionDat _section;

	private LabyrinthArea _area;

	private List<LabyrinthNodeOverride> _nodeOverrides;

	public LabyrinthSectionDat Section => _section ?? (_section = base.TheGame.Files.LabyrinthSections.GetByAddress(base.M.Read<long>(base.Address)));

	public LabyrinthArea Area => _area ?? (_area = base.TheGame.Files.LabyrinthAreas.GetByAddress(base.M.Read<long>(base.Address + 68)));

	public List<LabyrinthNodeOverride> NodeOverrides => _nodeOverrides ?? (_nodeOverrides = base.M.Read<DatArrayStruct>(base.Address + 92).ReadDatPtr(base.M).Select(base.TheGame.Files.LabyrinthNodeOverrides.GetByAddress)
		.ToList());

	public override string ToString()
	{
		return $"{Section?.Id} {Area?.Id} {base.Address:X}";
	}
}
