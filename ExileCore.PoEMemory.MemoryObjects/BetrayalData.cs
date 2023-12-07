using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.MemoryObjects;

public class BetrayalData : RemoteMemoryObject
{
	public BetrayalSyndicateLeadersData SyndicateLeadersData => GetObject<BetrayalSyndicateLeadersData>(base.M.Read<long>(base.Address + 720));

	public List<BetrayalSyndicateState> SyndicateStates
	{
		get
		{
			long num = base.M.Read<long>(base.Address + 800);
			return (from x in base.M.ReadStructsArray<BetrayalSyndicateState>(num, num + BetrayalSyndicateState.STRUCT_SIZE * 14, BetrayalSyndicateState.STRUCT_SIZE, this)
				where x.Target != null
				select x).ToList();
		}
	}

	public BetrayalEventData BetrayalEventData
	{
		get
		{
			long num = base.M.Read<long>(base.Address + 824, new int[1] { 816 });
			if (num != 0L)
			{
				return GetObject<BetrayalEventData>(num);
			}
			return null;
		}
	}
}
