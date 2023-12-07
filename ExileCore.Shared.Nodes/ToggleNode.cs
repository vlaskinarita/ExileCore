using System;
using Newtonsoft.Json;

namespace ExileCore.Shared.Nodes;

public sealed class ToggleNode
{
	[JsonIgnore]
	private bool value;

	public bool Value
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
					this.OnValueChanged?.Invoke(this, value);
				}
				catch (Exception ex)
				{
					DebugWindow.LogError($"Error in function that subscribed for: ToggleNode.OnValueChanged. {Environment.NewLine} {ex}", 10f);
				}
			}
		}
	}

	public event EventHandler<bool> OnValueChanged;

	public ToggleNode()
	{
	}

	public ToggleNode(bool value)
	{
		Value = value;
	}

	public void SetValueNoEvent(bool newValue)
	{
		value = newValue;
	}

	public static implicit operator bool(ToggleNode node)
	{
		return node.Value;
	}
}
