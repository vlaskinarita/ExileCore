using Newtonsoft.Json;

namespace ExileCore.Shared.AtlasHelper;

public class Pivot
{
	[JsonProperty("x")]
	public float X { get; set; }

	[JsonProperty("y")]
	public float Y { get; set; }
}
