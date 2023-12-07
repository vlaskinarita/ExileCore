namespace ExileCore.PoEMemory.FilesInMemory;

public class StampChoice : RemoteMemoryObject
{
	private string _id;

	private LakeRoom _room;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public LakeRoom Room => _room ?? (_room = base.TheGame.Files.LakeRooms.GetByAddress(base.M.Read<long>(base.Address + 8)));

	public override string ToString()
	{
		return $"{Id} {Room?.Description} ({base.Address:X})";
	}
}
