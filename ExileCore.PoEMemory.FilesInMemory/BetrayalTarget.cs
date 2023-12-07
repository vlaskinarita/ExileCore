using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.FilesInMemory;

public class BetrayalTarget : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public MonsterVariety MonsterVariety => base.TheGame.Files.MonsterVarieties.GetByAddress(base.M.Read<long>(base.Address + 24));

	public string Art => base.M.ReadStringU(base.M.Read<long>(base.Address + 56));

	public string FullName => base.M.ReadStringU(base.M.Read<long>(base.Address + 81));

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 97));

	public override string ToString()
	{
		return Name;
	}
}
