using ExileCore.PoEMemory.Models;

namespace ExileCore.PoEMemory.FilesInMemory;

public class QuestReward : RemoteMemoryObject
{
	private QuestRewardOffer _offer;

	private Character _character;

	private BaseItemType _reward;

	public QuestRewardOffer Offer => _offer ?? (_offer = base.TheGame.Files.QuestRewardOffers.GetByAddress(base.M.Read<long>(base.Address)));

	public Character Character => _character ?? (_character = base.TheGame.Files.Characters.GetByAddress(base.M.Read<long>(base.Address + 20)));

	public BaseItemType Reward => _reward ?? (_reward = base.TheGame.Files.BaseItemTypes.GetFromAddress(base.M.Read<long>(base.Address + 36)));

	public int RewardLevel => base.M.Read<int>(base.Address + 52);

	public override string ToString()
	{
		return $"{Offer?.Id} for {Character?.Name ?? Character?.Id} -> {Reward?.BaseName} ({base.Address:X})";
	}
}
