using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory.Labyrinth;

namespace ExileCore.PoEMemory.MemoryObjects;

public class LabyrinthRoom : RemoteMemoryObject
{
	private LabyrinthSecret _secret1;

	private LabyrinthSecret _secret2;

	private LabyrinthSectionLayout _section;

	private LabyrinthRoom[] _connections;

	internal Dictionary<long, LabyrinthRoom> RoomCache;

	public LabyrinthSecret Secret1 => _secret1 ?? (_secret1 = base.TheGame.Files.LabyrinthSecrets.GetByAddress(base.M.Read<long>(base.Address + 56)));

	public LabyrinthSecret Secret2 => _secret2 ?? (_secret2 = base.TheGame.Files.LabyrinthSecrets.GetByAddress(base.M.Read<long>(base.Address + 72)));

	public LabyrinthSectionLayout SectionLayout => _section ?? (_section = base.TheGame.Files.LabyrinthSectionLayouts.GetByAddress(base.M.Read<long>(base.Address + 40)));

	public LabyrinthRoom[] Connections => _connections ?? (_connections = (from x in base.M.ReadPointersArray(base.Address, base.Address + 32)
		select RoomCache?.GetValueOrDefault(x) into x
		where x != null
		select x).ToArray());

	public override string ToString()
	{
		string value = ((Connections.Length != 0) ? ("LinkedWith: " + string.Join(", ", Connections.Select((LabyrinthRoom x) => x.Address.ToString("X")).ToArray())) : "");
		return $"{base.Address:X}, Secret1: {Secret1?.Id ?? "None"}, Secret2: {Secret2?.Id ?? "None"}, {value}, Section: {SectionLayout}";
	}
}
