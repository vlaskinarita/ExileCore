using Newtonsoft.Json;

namespace ExileCore.Shared.AtlasHelper;

public class SpriteSourceSizeClass
{
	[JsonProperty("x")]
	public int X { get; set; }

	[JsonProperty("y")]
	public int Y { get; set; }

	[JsonProperty("w")]
	public int W { get; set; }

	[JsonProperty("h")]
	public int H { get; set; }
}
