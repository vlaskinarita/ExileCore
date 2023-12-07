using System;
using System.Numerics;
using GameOffsets.Native;
using SharpDX;

namespace ExileCore.Shared.Helpers;

public static class PoeMapExtension
{
	public const int TileToGridConversion = 23;

	public const int TileToWorldConversion = 250;

	public const float WorldToGridConversion = 0.092f;

	private const float Offset = 5.4347825f;

	public static System.Numerics.Vector2 GridToWorld(this System.Numerics.Vector2 v)
	{
		return v / 0.092f + new System.Numerics.Vector2(5.4347825f);
	}

	public static System.Numerics.Vector3 GridToWorld(this System.Numerics.Vector2 v, float z)
	{
		return new System.Numerics.Vector3(v.X / 0.092f + 5.4347825f, v.Y / 0.092f + 5.4347825f, z);
	}

	public static System.Numerics.Vector2 GridToWorld(this Vector2i v)
	{
		return v.ToVector2Num() / 0.092f + new System.Numerics.Vector2(5.4347825f);
	}

	public static System.Numerics.Vector3 GridToWorld(this Vector2i v, float z)
	{
		return new System.Numerics.Vector3((float)v.X / 0.092f + 5.4347825f, (float)v.Y / 0.092f + 5.4347825f, z);
	}

	public static System.Numerics.Vector2 WorldToGrid(this System.Numerics.Vector3 v)
	{
		return new System.Numerics.Vector2(MathF.Floor(v.X * 0.092f), MathF.Floor(v.Y * 0.092f));
	}

	public static System.Numerics.Vector2 WorldToGrid(this System.Numerics.Vector2 v)
	{
		return new System.Numerics.Vector2(MathF.Floor(v.X * 0.092f), MathF.Floor(v.Y * 0.092f));
	}

	public static Vector2i WorldToGridI(this System.Numerics.Vector3 v)
	{
		return new Vector2i((int)MathF.Floor(v.X * 0.092f), (int)MathF.Floor(v.Y * 0.092f));
	}

	public static Vector2i WorldToGridI(this System.Numerics.Vector2 v)
	{
		return new Vector2i((int)MathF.Floor(v.X * 0.092f), (int)MathF.Floor(v.Y * 0.092f));
	}

	[Obsolete]
	public static SharpDX.Vector2 GridToWorld(this SharpDX.Vector2 v)
	{
		return new SharpDX.Vector2(v.X / 0.092f + 5.4347825f, v.Y / 0.092f + 5.4347825f);
	}

	[Obsolete]
	public static SharpDX.Vector3 GridToWorld(this SharpDX.Vector2 v, float z)
	{
		return new SharpDX.Vector3(v.X / 0.092f + 5.4347825f, v.Y / 0.092f + 5.4347825f, z);
	}

	[Obsolete]
	public static SharpDX.Vector2 WorldToGrid(this SharpDX.Vector3 v)
	{
		return new SharpDX.Vector2((float)Math.Floor(v.X * 0.092f), (float)Math.Floor(v.Y * 0.092f));
	}

	[Obsolete]
	public static SharpDX.Vector2 WorldToGrid(this SharpDX.Vector2 v)
	{
		return new SharpDX.Vector2((float)Math.Floor(v.X * 0.092f), (float)Math.Floor(v.Y * 0.092f));
	}
}
