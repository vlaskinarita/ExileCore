using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Elements;

public class ExpeditionDetonatorInfo : StructuredRemoteMemoryObject<ExpeditionDetonatorInfoOffsets>
{
	public bool IsExplosivePlacementActive => base.Structure.PlacementMarkerPtr != 0;

	public Vector2i[] PlacedExplosiveGridPositions => base.M.ReadStdVector<Vector2i>(base.Structure.PlacedExplosives);

	public int TotalExplosiveCount => base.Structure.TotalExplosiveCount;

	public int PlacedExplosiveCount => (int)base.Structure.PlacedExplosives.ElementCount<Vector2i>();

	public int RemainingExplosiveCount => TotalExplosiveCount - PlacedExplosiveCount;

	public Vector2i DetonatorGridPosition => base.Structure.DetonatorGridPosition;

	public Vector2i PlacementIndicatorGridPosition => base.Structure.PlacementIndicatorGridPosition;
}
