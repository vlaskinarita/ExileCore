using System;

namespace ExileCore.Shared;

public class BuildError
{
	public string File { get; set; }

	public DateTime Timestamp { get; set; }

	public int LineNumber { get; set; }

	public int ColumnNumber { get; set; }

	public string Code { get; set; }

	public string Message { get; set; }

	public override string ToString()
	{
		return $"[{Timestamp}, {File}({LineNumber}, {ColumnNumber})] {Code}: {Message}";
	}
}
