namespace ExileCore.PoEMemory.FilesInMemory.Metamorph;

public class MetamorphRewardTypeItemsClient : RemoteMemoryObject
{
	public MetamorphRewardType RewardType => base.TheGame.Files.MetamorphRewardTypes.GetByAddress(base.M.Read<long>(base.Address + 8));

	public int Unknown => base.M.Read<int>(base.Address + 16);

	public string Description => base.M.ReadStringU(base.M.Read<long>(base.Address + 20), 255);

	public override string ToString()
	{
		return RewardType.Id + ": " + Description;
	}
}
