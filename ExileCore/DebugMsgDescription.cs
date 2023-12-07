using System;
using System.Numerics;
using SharpDX;

namespace ExileCore;

public class DebugMsgDescription
{
	public string Msg { get; set; }

	public DateTime Time { get; set; }

	public System.Numerics.Vector4 ColorV4 { get; set; }

	public Color Color { get; set; }

	public int Count { get; set; }
}
