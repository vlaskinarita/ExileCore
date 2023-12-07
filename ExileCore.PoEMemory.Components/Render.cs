using System;
using System.Numerics;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using GameOffsets;
using SharpDX;

namespace ExileCore.PoEMemory.Components;

public class Render : Component
{
	private readonly CachedValue<RenderComponentOffsets> _cachedValue;

	public RenderComponentOffsets RenderStruct => _cachedValue.Value;

	public float X => PosNum.X;

	public float Y => PosNum.Y;

	public float Z => PosNum.Z;

	[Obsolete]
	public SharpDX.Vector3 Pos => RenderStruct.Pos.ToSharpDx();

	public System.Numerics.Vector3 PosNum => RenderStruct.Pos;

	[Obsolete]
	public SharpDX.Vector3 InteractCenter => InteractCenterNum.ToSharpDx();

	public System.Numerics.Vector3 InteractCenterNum => PosNum + BoundsNum / 2f;

	public float Height
	{
		get
		{
			if (!(RenderStruct.Height > 0.01f))
			{
				return 0f;
			}
			return RenderStruct.Height;
		}
	}

	public string Name => RemoteMemoryObject.Cache.StringCache.Read($"{"Render"}{RenderStruct.Name.buf}", () => RenderStruct.Name.ToString(base.M));

	public string NameNoCache => RenderStruct.Name.ToString(base.M);

	[Obsolete]
	public SharpDX.Vector3 Rotation => RenderStruct.Rotation.ToSharpDx();

	public System.Numerics.Vector3 RotationNum => RenderStruct.Rotation;

	[Obsolete]
	public SharpDX.Vector3 Bounds => RenderStruct.Bounds.ToSharpDx();

	public System.Numerics.Vector3 BoundsNum => RenderStruct.Bounds;

	[Obsolete]
	public SharpDX.Vector3 MeshRoration => RenderStruct.Rotation.ToSharpDx();

	public System.Numerics.Vector3 MeshRotationNum => RenderStruct.Rotation;

	public float RotationInDegrees => RenderStruct.Rotation.X * (180f / (float)Math.PI);

	public float TerrainHeight
	{
		get
		{
			if (!(RenderStruct.Height > 0.01f))
			{
				return 0f;
			}
			return RenderStruct.Height;
		}
	}

	public Render()
	{
		_cachedValue = new FrameCache<RenderComponentOffsets>(() => base.M.Read<RenderComponentOffsets>(base.Address));
	}
}
