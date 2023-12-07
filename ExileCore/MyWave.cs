using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace ExileCore;

public class MyWave
{
	public AudioBuffer Buffer { get; set; }

	public uint[] DecodedPacketsInfo { get; set; }

	public WaveFormat WaveFormat { get; set; }
}
