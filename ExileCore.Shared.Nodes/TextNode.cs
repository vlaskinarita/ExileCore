using System;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore.Shared.Nodes;

public class TextNode
{
	[JsonIgnore]
	public Action OnValueChanged = delegate
	{
	};

	private string value;

	public string Value
	{
		get
		{
			return value;
		}
		set
		{
			if (this.value != value)
			{
				this.value = value;
				try
				{
					OnValueChanged();
				}
				catch (Exception)
				{
					DebugWindow.LogMsg("Error in function that subscribed for: TextNode.OnValueChanged", 10f, Color.Red);
				}
			}
		}
	}

	public TextNode()
	{
	}

	public TextNode(string value)
	{
		Value = value;
	}

	public void SetValueNoEvent(string newValue)
	{
		value = newValue;
	}

	public static implicit operator string(TextNode node)
	{
		return node.Value;
	}

	public static implicit operator TextNode(string value)
	{
		return new TextNode(value);
	}
}
