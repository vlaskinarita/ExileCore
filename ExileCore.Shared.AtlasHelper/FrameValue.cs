using Newtonsoft.Json;

namespace ExileCore.Shared.AtlasHelper;

public class FrameValue
{
	[JsonProperty("frame")]
	public SpriteSourceSizeClass Frame { get; set; }

	[JsonProperty("rotated")]
	public bool Rotated { get; set; }

	[JsonProperty("trimmed")]
	public bool Trimmed { get; set; }

	[JsonProperty("spriteSourceSize")]
	public SpriteSourceSizeClass SpriteSourceSize { get; set; }

	[JsonProperty("sourceSize")]
	public Size SourceSize { get; set; }

	[JsonProperty("pivot")]
	public Pivot Pivot { get; set; }
}
