using System;

namespace ExileCore.Shared.Nodes;

public sealed class FileNode
{
	private string value;

	public string Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			this.OnFileChanged?.Invoke(this, value);
		}
	}

	public event EventHandler<string> OnFileChanged;

	public FileNode()
	{
	}

	public FileNode(string value)
	{
		Value = value;
	}

	public static implicit operator string(FileNode node)
	{
		return node.Value;
	}

	public static implicit operator FileNode(string value)
	{
		return new FileNode(value);
	}
}
