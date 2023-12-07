using System;
using System.Numerics;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using GameOffsets;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects;

public class Camera : RemoteMemoryObject
{
	private readonly CachedValue<CameraOffsets> _cachedValue;

	public CameraOffsets CameraOffsets => _cachedValue.Value;

	public int Width => CameraOffsets.Width;

	public int Height => CameraOffsets.Height;

	private float HalfWidth { get; set; }

	private float HalfHeight { get; set; }

	[Obsolete]
	public SharpDX.Vector2 Size => new SharpDX.Vector2(Width, Height);

	public System.Numerics.Vector2 SizeNum => new System.Numerics.Vector2(Width, Height);

	public float ZFar => CameraOffsets.ZFar;

	[Obsolete]
	public SharpDX.Vector3 Position => CameraOffsets.Position.ToSharpDx();

	public System.Numerics.Vector3 PositionNum => CameraOffsets.Position;

	public string PositionString => PositionNum.ToString();

	private Matrix4x4 Matrix => CameraOffsets.MatrixBytes;

	public Camera()
	{
		_cachedValue = new FrameCache<CameraOffsets>(() => base.M.Read<CameraOffsets>(base.Address));
		_cachedValue.OnUpdate += delegate(CameraOffsets offsets)
		{
			HalfHeight = (float)offsets.Height * 0.5f;
			HalfWidth = (float)offsets.Width * 0.5f;
		};
	}

	public System.Numerics.Vector2 WorldToScreen(System.Numerics.Vector3 vec)
	{
		try
		{
			System.Numerics.Vector4 vector = new System.Numerics.Vector4(vec, 1f);
			vector = System.Numerics.Vector4.Transform(vector, Matrix);
			vector = System.Numerics.Vector4.Divide(vector, vector.W);
			System.Numerics.Vector2 result = default(System.Numerics.Vector2);
			result.X = (vector.X + 1f) * HalfWidth;
			result.Y = (1f - vector.Y) * HalfHeight;
			return result;
		}
		catch (Exception value)
		{
			Core.Logger?.Error($"Camera WorldToScreen {value}");
		}
		return System.Numerics.Vector2.Zero;
	}

	[Obsolete]
	public SharpDX.Vector2 WorldToScreen(SharpDX.Vector3 vec)
	{
		return WorldToScreen(vec.ToVector3Num()).ToSharpDx();
	}
}
