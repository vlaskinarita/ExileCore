namespace ExileCore.PoEMemory.FilesInMemory.Labyrinth;

public class LabyrinthSectionDat : RemoteMemoryObject
{
	private string _id;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public override string ToString()
	{
		return $"{Id} {base.Address:X}";
	}
}
