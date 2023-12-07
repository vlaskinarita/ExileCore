using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects;

public class AreaTemplate : RemoteMemoryObject
{
	public string RawName => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public int Act => base.M.Read<int>(base.Address + 16);

	public bool IsTown => base.M.Read<byte>(base.Address + 20) == 1;

	public bool HasWaypoint => base.M.Read<byte>(base.Address + 21) == 1;

	public int NominalLevel => base.M.Read<int>(base.Address + 38);

	public int MonsterLevel => base.M.Read<int>(base.Address + 38);

	public int WorldAreaId => base.M.Read<int>(base.Address + 42);

	public int CorruptedAreasVariety => base.M.Read<int>(base.Address + 251);

	public List<WorldArea> PossibleCorruptedAreas => _PossibleCorruptedAreas(base.Address + 259, CorruptedAreasVariety);

	private List<WorldArea> _PossibleCorruptedAreas(long address, int count)
	{
		List<WorldArea> list = new List<WorldArea>();
		for (int i = 0; i < count; i++)
		{
			list.Add(GetObject<WorldArea>(address + i * 8));
		}
		return list;
	}
}
