using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class QuestState : RemoteMemoryObject
{
	public QuestStateOffsets QuestStateOffsets => base.M.Read<QuestStateOffsets>(base.Address);

	public long QuestPtr => QuestStateOffsets.QuestAddress;

	public Quest Quest => base.TheGame.Files.Quests.GetByAddress(QuestPtr);

	public int QuestStateId => QuestStateOffsets.QuestStateId;

	public string QuestStateText => base.M.ReadStringU(QuestStateOffsets.QuestStateTextAddress);

	public string QuestProgressText => base.M.ReadStringU(QuestStateOffsets.QuestProgressTextAddress);

	public override string ToString()
	{
		return $"Id: {QuestStateId}, Quest.Id: {Quest.Id}, ProgressText {QuestProgressText}, QuestName: {Quest.Name}";
	}
}
