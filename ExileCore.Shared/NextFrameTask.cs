using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ExileCore.Shared;

public class NextFrameTask
{
	public class NextFrameAwaiter : INotifyCompletion
	{
		private static readonly ConcurrentQueue<Action> Continuations = new ConcurrentQueue<Action>();

		public bool IsCompleted => false;

		public void GetResult()
		{
		}

		public static void SetNextFrame()
		{
			Action result;
			while (Continuations.TryDequeue(out result))
			{
				result();
			}
		}

		public void OnCompleted(Action completion)
		{
			Continuations.Enqueue(completion);
		}
	}

	private NextFrameAwaiter Awaiter { get; } = new NextFrameAwaiter();


	public NextFrameAwaiter GetAwaiter()
	{
		return Awaiter;
	}
}
