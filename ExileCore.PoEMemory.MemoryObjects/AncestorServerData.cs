using System.Collections.Generic;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.MemoryObjects;

public class AncestorServerData : RemoteMemoryObject
{
	private const int FightDataOffset = 456;

	public List<AncestorFightOption> Options
	{
		get
		{
			StdVector stdVector = base.M.Read<StdVector>(base.Address + 456);
			return base.M.ReadStructsArray<AncestorFightOption>(stdVector.First, stdVector.Last, 48, null);
		}
	}
}
