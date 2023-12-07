using System;
using SharpDX;

namespace ExileCore;

public static class WorldPositionExtensions
{
	private const float MarsEllipticOrbit = 0.092f;

	private const float Offset = 5.434783f;

	[Obsolete]
	public static Vector2 GridToWorld(this Vector2 v)
	{
		return new Vector2(v.X / 0.092f + 5.434783f, v.Y / 0.092f + 5.434783f);
	}

	[Obsolete]
	public static Vector3 GridToWorld(this Vector2 v, float z)
	{
		return new Vector3(v.X / 0.092f + 5.434783f, v.Y / 0.092f + 5.434783f, z);
	}

	[Obsolete]
	public static Vector2 WorldToGrid(this Vector3 v)
	{
		return new Vector2((float)Math.Floor(v.X * 0.092f), (float)Math.Floor(v.Y * 0.092f));
	}
}
