namespace ExileCore.PoEMemory.FilesInMemory.Sanctum;

public class SanctumDeferredReward : RemoteMemoryObject
{
	private string _id;

	private int? _count;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public SanctumDeferredRewardCategory RewardCategory => base.TheGame.Files.SanctumDeferredRewardCategories.GetByAddress(base.M.Read<long>(base.Address + 32));

	public int Count
	{
		get
		{
			int valueOrDefault = _count.GetValueOrDefault();
			if (!_count.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 48);
				_count = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public override string ToString()
	{
		return $"{Count}x {RewardCategory?.CurrencyName} ({Id})";
	}
}
