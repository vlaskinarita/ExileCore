using System.Text;

namespace ExileCore.PoEMemory.Models;

public class BaseItemType
{
	public string Metadata { get; set; }

	public string ClassName { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public int DropLevel { get; set; }

	public string BaseName { get; set; }

	public string[] Tags { get; set; }

	public string[] MoreTagsFromPath { get; set; }

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Tags: ");
		string[] tags = Tags;
		foreach (string value in tags)
		{
			stringBuilder.Append(value);
			stringBuilder.Append(" ");
		}
		stringBuilder.Append("More Tags: ");
		tags = MoreTagsFromPath;
		foreach (string value2 in tags)
		{
			stringBuilder.Append(value2);
			stringBuilder.Append(" ");
		}
		return stringBuilder.ToString();
	}
}
