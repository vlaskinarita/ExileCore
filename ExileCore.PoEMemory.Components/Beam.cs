using System;
using System.Numerics;
using SharpDX;

namespace ExileCore.PoEMemory.Components;

public class Beam : Component
{
	private const int BeamStartOffset = 80;

	private const int BeamEndOffset = 92;

	[Obsolete]
	public SharpDX.Vector3 BeamStart => base.M.Read<SharpDX.Vector3>(base.Address + 80);

	[Obsolete]
	public SharpDX.Vector3 BeamEnd => base.M.Read<SharpDX.Vector3>(base.Address + 92);

	public System.Numerics.Vector3 BeamStartNum => base.M.Read<System.Numerics.Vector3>(base.Address + 80);

	public System.Numerics.Vector3 BeamEndNum => base.M.Read<System.Numerics.Vector3>(base.Address + 92);

	public int Unknown1 => base.M.Read<int>(base.Address + 64);

	public int Unknown2 => base.M.Read<int>(base.Address + 68);
}
