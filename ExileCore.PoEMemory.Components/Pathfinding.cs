using System;
using System.Collections.Generic;
using ExileCore.Shared.Cache;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class Pathfinding : Component
{
	private readonly CachedValue<PathfindingComponentOffsets> _cachedValue;

	private PathfindingComponentOffsets Offsets => _cachedValue.Value;

	public Vector2i TargetMovePos => default(Vector2i);

	public Vector2i PreviousMovePos => default(Vector2i);

	public Vector2i WantMoveToPosition => Offsets.WantMoveToPosition;

	public bool IsMoving => Offsets.DestinationNodes > 0;

	public int DestinationNodes => Offsets.DestinationNodes;

	public float StayTime => Offsets.StayTime;

	public IList<Vector2i> PathingNodes
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			int destinationNodes = Offsets.DestinationNodes;
			if (destinationNodes < 0 || destinationNodes > 30)
			{
				return new List<Vector2i>();
			}
			int num = destinationNodes * 4 * 2;
			byte[] value = base.M.ReadMem(base.Address + PathfindingComponentOffsets.PathNodeStart, num);
			List<Vector2i> list = new List<Vector2i>();
			for (int i = 0; i < num; i += 8)
			{
				int x = BitConverter.ToInt32(value, i);
				int y = BitConverter.ToInt32(value, i + 4);
				list.Add(new Vector2i(x, y));
			}
			list.Reverse();
			return list;
		}
	}

	public Pathfinding()
	{
		_cachedValue = new FrameCache<PathfindingComponentOffsets>(() => base.M.Read<PathfindingComponentOffsets>(base.Address));
	}
}
