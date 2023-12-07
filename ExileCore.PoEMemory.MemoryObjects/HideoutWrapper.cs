namespace ExileCore.PoEMemory.MemoryObjects;

public class HideoutWrapper : RemoteMemoryObject
{
	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public WorldArea WorldArea1 => base.TheGame.Files.WorldAreas.GetByAddress(base.M.Read<long>(base.Address + 8));

	public WorldArea WorldArea2 => base.TheGame.Files.WorldAreas.GetByAddress(base.M.Read<long>(base.Address + 48));

	public WorldArea WorldArea3 => base.TheGame.Files.WorldAreas.GetByAddress(base.M.Read<long>(base.Address + 64));
}
