using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExileCore.Shared.AtlasHelper;

public class AtlasConfigData
{
	[JsonProperty("frames")]
	public Dictionary<string, FrameValue> Frames { get; set; }

	[JsonProperty("meta")]
	public Meta Meta { get; set; }
}
