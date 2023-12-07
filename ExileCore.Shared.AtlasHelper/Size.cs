using Newtonsoft.Json;

namespace ExileCore.Shared.AtlasHelper;

public class Size
{
	[JsonProperty("w")]
	public int W { get; set; }

	[JsonProperty("h")]
	public int H { get; set; }
}
