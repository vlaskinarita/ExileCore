using System;

namespace ExileCore.Shared;

public struct CoroutineDetails
{
	public string Name { get; set; }

	public string OwnerName { get; set; }

	public long Ticks { get; set; }

	public DateTime Started { get; set; }

	public DateTime Finished { get; set; }

	public CoroutineDetails(string name, string ownerName, long ticks, DateTime started, DateTime finished)
	{
		Name = name;
		OwnerName = ownerName;
		Ticks = ticks;
		Started = started;
		Finished = finished;
	}
}
