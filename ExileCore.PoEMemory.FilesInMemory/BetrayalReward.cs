namespace ExileCore.PoEMemory.FilesInMemory;

public class BetrayalReward : RemoteMemoryObject
{
	public BetrayalJob Job => base.TheGame.Files.BetrayalJobs.GetByAddress(base.M.Read<long>(base.Address));

	public BetrayalTarget Target => base.TheGame.Files.BetrayalTargets.GetByAddress(base.M.Read<long>(base.Address + 16));

	public BetrayalRank Rank => base.TheGame.Files.BetrayalRanks.GetByAddress(base.M.Read<long>(base.Address + 32));

	public string Reward => base.M.ReadStringU(base.M.Read<long>(base.Address + 48));

	public override string ToString()
	{
		return Reward;
	}
}
