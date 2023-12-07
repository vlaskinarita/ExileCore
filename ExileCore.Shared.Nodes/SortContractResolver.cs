using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ExileCore.Shared.Nodes;

public sealed class SortContractResolver : DefaultContractResolver
{
	private const int MAX_PROPERTIES_PER_CONTRACT = 1000;

	protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
	{
		return (from member in GetSerializableMembers(type) ?? throw new JsonSerializationException("Null collection of serializable members returned.")
			select CreateProperty(member, memberSerialization) into x
			where x != null
			orderby 1000 * GetTypeDepth(x.DeclaringType) + x.Order.GetValueOrDefault()
			select x).ToList();
	}

	private static int GetTypeDepth(Type type)
	{
		int num = 0;
		while ((type = type.BaseType) != null)
		{
			num++;
		}
		return num;
	}
}
