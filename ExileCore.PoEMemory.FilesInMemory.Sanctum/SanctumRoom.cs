namespace ExileCore.PoEMemory.FilesInMemory.Sanctum;

public class SanctumRoom : RemoteMemoryObject
{
	private SanctumRoomType _roomType;

	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Arm => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public string Teleports => base.M.ReadStringU(base.M.Read<long>(base.Address + 32));

	public SanctumRoomType RoomType => _roomType ?? (_roomType = base.TheGame.Files.SanctumRoomTypes.GetByAddress(base.M.Read<long>(base.Address + 16)));

	public override string ToString()
	{
		return Id;
	}
}
