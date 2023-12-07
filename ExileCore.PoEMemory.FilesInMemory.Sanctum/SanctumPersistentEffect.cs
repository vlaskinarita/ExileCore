namespace ExileCore.PoEMemory.FilesInMemory.Sanctum;

public class SanctumPersistentEffect : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string ReadableName => base.M.ReadStringU(base.M.Read<long>(base.Address + 40));

	public string Description => base.M.ReadStringU(base.M.Read<long>(base.Address + 105));

	public override string ToString()
	{
		return Id + " (" + ReadableName + ")";
	}
}
