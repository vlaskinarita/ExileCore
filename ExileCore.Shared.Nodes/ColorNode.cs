using System;
using System.Drawing;
using SharpDX;

namespace ExileCore.Shared.Nodes;

public sealed class ColorNode
{
	private SharpDX.Color _value;

	public string Hex { get; private set; }

	public SharpDX.Color Value
	{
		get
		{
			return _value;
		}
		set
		{
			if (_value != value)
			{
				Hex = ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B));
				_value = value;
				try
				{
					this.OnValueChanged?.Invoke(this, value);
				}
				catch (Exception value2)
				{
					DebugWindow.LogMsg($"Error in function that subscribed for: {"ColorNode"}.{"OnValueChanged"}: {value2}", 10f, SharpDX.Color.Red);
				}
			}
		}
	}

	public event EventHandler<SharpDX.Color> OnValueChanged;

	public ColorNode()
	{
	}

	public ColorNode(uint color)
	{
		Value = SharpDX.Color.FromAbgr(color);
	}

	public ColorNode(SharpDX.Color color)
	{
		Value = color;
	}

	public static implicit operator SharpDX.Color(ColorNode node)
	{
		return node.Value;
	}

	public static implicit operator ColorNode(uint value)
	{
		return new ColorNode(value);
	}

	public static implicit operator ColorNode(SharpDX.Color value)
	{
		return new ColorNode(value);
	}

	public static implicit operator ColorNode(ColorBGRA value)
	{
		return new ColorNode(value);
	}
}
