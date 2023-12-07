using System.Numerics;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class Movement : Component
{
	public Vector2 MovingToGridPosNum
	{
		get
		{
			if (base.Address == 0L)
			{
				return new Vector2(0f, 0f);
			}
			return base.M.Read<Vector2i>(base.Address + 60).ToVector2Num();
		}
	}
}
