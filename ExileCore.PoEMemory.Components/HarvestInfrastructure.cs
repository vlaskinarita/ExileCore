using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.Components;

public class HarvestInfrastructure : Component
{
	public unsafe List<HarvestInfrastructureMod> CraftMods
	{
		get
		{
			long startAddress = base.M.Read<long>(base.Address + 32);
			long endAddress = base.M.Read<long>(base.Address + 40);
			return (from x in base.M.ReadStructsArray<HarvestInfrastructureModUnmanaged>(startAddress, endAddress, sizeof(HarvestInfrastructureModUnmanaged))
				select new HarvestInfrastructureMod(x, base.M)).ToList();
		}
	}
}
