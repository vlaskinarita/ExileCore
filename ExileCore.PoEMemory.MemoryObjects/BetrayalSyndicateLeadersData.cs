using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects;

public class BetrayalSyndicateLeadersData : RemoteMemoryObject
{
	public List<BetrayalSyndicateState> Leaders => new List<BetrayalSyndicateState>
	{
		ReadObjectAt<BetrayalSyndicateState>(0),
		ReadObjectAt<BetrayalSyndicateState>(8),
		ReadObjectAt<BetrayalSyndicateState>(16),
		ReadObjectAt<BetrayalSyndicateState>(24)
	};
}
