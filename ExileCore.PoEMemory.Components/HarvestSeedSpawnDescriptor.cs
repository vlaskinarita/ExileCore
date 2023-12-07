using ExileCore.PoEMemory.FilesInMemory.Harvest;

namespace ExileCore.PoEMemory.Components;

public class HarvestSeedSpawnDescriptor : RemoteMemoryObject
{
	private HarvestSeed _seed;

	public HarvestSeed Seed => _seed ?? (_seed = base.TheGame.Files.HarvestSeeds.GetByAddress(base.M.Read<long>(base.Address)));

	public int Count => base.M.Read<int>(base.Address + 16);
}
