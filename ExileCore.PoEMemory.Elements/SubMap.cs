using System;
using System.Numerics;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using SharpDX;

namespace ExileCore.PoEMemory.Elements;

public class SubMap : Element
{
	private readonly CachedValue<float> _mapScale;

	private readonly CachedValue<System.Numerics.Vector2> _mapCenter;

	public const double CameraAngle = 0.6754424205218056;

	public static readonly float CameraAngleCos = (float)Math.Cos(0.6754424205218056);

	public static readonly float CameraAngleSin = (float)Math.Sin(0.6754424205218056);

	[Obsolete]
	public SharpDX.Vector2 Shift => base.M.Read<SharpDX.Vector2>(base.Address + 616);

	public System.Numerics.Vector2 ShiftNum => base.M.Read<System.Numerics.Vector2>(base.Address + 616);

	[Obsolete]
	public SharpDX.Vector2 DefaultShift => base.M.Read<SharpDX.Vector2>(base.Address + 624);

	public System.Numerics.Vector2 DefaultShiftNum => base.M.Read<System.Numerics.Vector2>(base.Address + 624);

	public float Zoom => base.M.Read<float>(base.Address + 684);

	public float MapScale => _mapScale.Value;

	public System.Numerics.Vector2 MapCenter => _mapCenter.Value;

	public SubMap()
	{
		_mapScale = new FrameCache<float>(() => (float)base.TheGame.IngameState.Camera.Height / 677f * Zoom);
		_mapCenter = new FrameCache<System.Numerics.Vector2>(() => base.GetClientRectCache.TopLeft.ToVector2Num() + ShiftNum + DefaultShiftNum);
	}
}
