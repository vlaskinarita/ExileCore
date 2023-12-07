namespace ExileCore.PoEMemory.FilesInMemory.Sanctum;

public class SanctumRoomType : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public override string ToString()
	{
		return Id;
	}
}
