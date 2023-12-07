using System;
using System.Numerics;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;
using SharpDX;

namespace ExileCore.PoEMemory.Components;

public class Positioned : Component
{
	private readonly CachedValue<PositionedComponentOffsets> _cachedValue;

	public PositionedComponentOffsets PositionedStruct => _cachedValue.Value;

	public new long OwnerAddress => PositionedStruct.OwnerAddress;

	public int GridX => GridPosition.X;

	public int GridY => GridPosition.Y;

	[Obsolete]
	public SharpDX.Vector2 GridPos => new SharpDX.Vector2(GridX, GridY);

	public System.Numerics.Vector2 GridPosNum => new System.Numerics.Vector2(GridX, GridY);

	public Vector2i GridPosI => new Vector2i(GridX, GridY);

	[Obsolete]
	public SharpDX.Vector2 WorldPos => PositionedStruct.WorldPosition.ToSharpDx();

	public System.Numerics.Vector2 WorldPosNum => PositionedStruct.WorldPosition;

	public System.Numerics.Vector2 TravelTarget => PositionedStruct.TravelStart + PositionedStruct.TravelOffset;

	public System.Numerics.Vector2 TravelStart => PositionedStruct.TravelStart;

	public float TravelProgress => PositionedStruct.TravelProgress;

	public Vector2i GridPosition => PositionedStruct.GridPosition;

	public float Rotation => PositionedStruct.Rotation;

	public float WorldX => WorldPosNum.X;

	public float WorldY => WorldPosNum.Y;

	public float RotationDeg => Rotation * (180f / (float)Math.PI);

	public byte Reaction => PositionedStruct.Reaction;

	public int Size => PositionedStruct.Size;

	public float Scale => PositionedStruct.Scale;

	public Positioned()
	{
		_cachedValue = new FrameCache<PositionedComponentOffsets>(() => base.M.Read<PositionedComponentOffsets>(base.Address));
	}
}
