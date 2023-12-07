using SharpDX;

namespace ExileCore.Shared;

public class HudTexture
{
	public string FileName { get; set; }

	public RectangleF UV { get; set; } = new RectangleF(0f, 0f, 1f, 1f);


	public float Size { get; set; } = 13f;


	public Color Color { get; set; } = Color.White;


	public HudTexture()
	{
	}

	public HudTexture(string fileName)
	{
		FileName = fileName;
	}
}
