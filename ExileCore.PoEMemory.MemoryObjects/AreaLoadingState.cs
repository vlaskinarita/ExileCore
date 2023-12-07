using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class AreaLoadingState : GameState
{
	private AreaLoadingStateOffsets Data => base.M.Read<AreaLoadingStateOffsets>(base.Address);

	public bool IsLoading => Data.IsLoading == 1;

	public uint TotalLoadingScreenTimeMs => Data.TotalLoadingScreenTimeMs;

	public string AreaName => base.M.ReadStringU(Data.AreaName);

	public override string ToString()
	{
		return $"{AreaName}, IsLoading: {IsLoading}";
	}
}
