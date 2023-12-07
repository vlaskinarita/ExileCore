namespace ExileCore.PoEMemory.FilesInMemory;

public class BetrayalRank : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public int Unknown => base.M.Read<int>(base.Address + 16);

	public string Art => base.M.ReadStringU(base.M.Read<long>(base.Address + 20));

	public int RankInt => Id switch
	{
		"Rank1" => 1, 
		"Rank2" => 2, 
		"Rank3" => 3, 
		_ => 0, 
	};

	public override string ToString()
	{
		return Name;
	}
}
