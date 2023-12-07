using System;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore.Shared.Nodes;

public sealed class RangeNode<T> where T : struct
{
	private T _value;

	public T Value
	{
		get
		{
			return _value;
		}
		set
		{
			if (!value.Equals(_value))
			{
				_value = value;
				try
				{
					this.OnValueChanged?.Invoke(this, value);
				}
				catch (Exception)
				{
					DebugWindow.LogMsg("Error in function that subscribed for: RangeNode.OnValueChanged", 10f, Color.Red);
				}
			}
		}
	}

	[JsonIgnore]
	public T Min { get; set; }

	[JsonIgnore]
	public T Max { get; set; }

	public event EventHandler<T> OnValueChanged;

	public RangeNode()
	{
	}

	public RangeNode(T value, T min, T max)
	{
		Value = value;
		Min = min;
		Max = max;
	}

	public static implicit operator T(RangeNode<T> node)
	{
		return node.Value;
	}
}
