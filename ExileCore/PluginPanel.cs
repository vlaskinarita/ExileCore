using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Enums;
using SharpDX;

namespace ExileCore;

public class PluginPanel
{
	private readonly List<Func<bool>> settings = new List<Func<bool>>();

	public bool Used => settings.Any((Func<bool> x) => x());

	public Vector2 StartDrawPoint { get; set; }

	public Vector2 Margin { get; }

	public PluginPanel(Vector2 startDrawPoint, Direction direction = Direction.Down)
		: this(direction)
	{
		StartDrawPoint = startDrawPoint;
	}

	public PluginPanel(Direction direction = Direction.Down)
	{
		Margin = new Vector2(0f, 0f);
	}

	public void WantUse(Func<bool> enabled)
	{
		settings.Add(enabled);
	}
}
