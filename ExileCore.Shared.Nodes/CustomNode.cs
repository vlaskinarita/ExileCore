using System;

namespace ExileCore.Shared.Nodes;

public class CustomNode
{
	public Action DrawDelegate;

	public CustomNode()
	{
	}

	public CustomNode(Action drawDelegate)
	{
		DrawDelegate = drawDelegate;
	}
}
