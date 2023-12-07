using System;
using System.Diagnostics;

namespace ExileCore;

[DebuggerDisplay("Name: {Name}, Elapsed: {ElapsedMs}, Completed: {IsCompleted}, Failed: {IsFailed}")]
public class Job
{
	public volatile bool IsCompleted;

	public volatile bool IsFailed;

	public volatile bool IsStarted;

	public Action Work { get; set; }

	public string Name { get; set; }

	public ThreadUnit WorkingOnThread { get; set; }

	public double ElapsedMs { get; set; }

	public Job(string name, Action work)
	{
		Name = name;
		Work = work;
	}
}
