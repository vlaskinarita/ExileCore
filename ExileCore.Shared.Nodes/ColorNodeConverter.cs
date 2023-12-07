using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExileCore.Shared.Nodes;

public class ColorNodeConverter : CustomCreationConverter<ColorNode>
{
	public override bool CanWrite => true;

	public override bool CanRead => true;

	public override ColorNode Create(Type objectType)
	{
		return new ColorNode();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (!uint.TryParse(reader.Value.ToString(), NumberStyles.HexNumber, null, out var result))
		{
			return Create(objectType);
		}
		return new ColorNode(result);
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		ColorNode colorNode = (ColorNode)value;
		serializer.Serialize(writer, $"{colorNode.Value.ToAbgr():x8}");
	}
}
