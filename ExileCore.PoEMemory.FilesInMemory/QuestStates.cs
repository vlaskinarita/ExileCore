using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class QuestStates : UniversalFileWrapper<QuestState>
{
	private Dictionary<(string, int), QuestState> _questStatesDictionary;

	public QuestStates(IMemory m, Func<long> address)
		: base(m, address)
	{
	}

	public QuestState GetQuestState(string questId, int stateId)
	{
		if (_questStatesDictionary == null)
		{
			_questStatesDictionary = base.EntriesList.ToDictionary((QuestState x) => (x.Quest.Id.ToLowerInvariant(), x.QuestStateId));
		}
		return _questStatesDictionary.GetValueOrDefault((questId.ToLowerInvariant(), stateId));
	}
}
