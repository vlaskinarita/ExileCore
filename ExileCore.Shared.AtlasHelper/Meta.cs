using System;
using Newtonsoft.Json;

namespace ExileCore.Shared.AtlasHelper;

public class Meta
{
	[JsonProperty("app")]
	public Uri App { get; set; }

	[JsonProperty("version")]
	public string Version { get; set; }

	[JsonProperty("image")]
	public string Image { get; set; }

	[JsonProperty("format")]
	public string Format { get; set; }

	[JsonProperty("size")]
	public Size Size { get; set; }

	[JsonProperty("scale")]
	public long Scale { get; set; }
}
