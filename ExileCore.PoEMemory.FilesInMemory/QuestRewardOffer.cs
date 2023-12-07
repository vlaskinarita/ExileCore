using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.FilesInMemory;

public class QuestRewardOffer : RemoteMemoryObject
{
	private string _id;

	private Quest _quest;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public Quest Quest => _quest ?? (_quest = base.TheGame.Files.Quests.GetByAddress(base.M.Read<long>(base.Address + 8)));

	public override string ToString()
	{
		return $"{Id} ({base.Address:X})";
	}
}
