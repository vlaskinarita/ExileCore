using System;
using System.Threading;

namespace ExileCore.Shared;

public static class TaskUtils
{
	public static async SyncTask<bool> CheckEveryFrame(Func<bool> condition, CancellationToken cancellationToken)
	{
		while (true)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return false;
			}
			if (condition())
			{
				break;
			}
			await NextFrame();
		}
		return true;
	}

	public static async SyncTask<bool> CheckEveryFrameWithThrow(Func<bool> condition, CancellationToken cancellationToken)
	{
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (condition())
			{
				break;
			}
			await NextFrame();
		}
		return true;
	}

	public static SyncTask<T> RunOrRestart<T>(ref SyncTask<T> oldTask, Func<SyncTask<T>> taskProvider)
	{
		oldTask?.GetAwaiter().PumpEvents();
		ClearIfCompleted(ref oldTask, taskProvider);
		return oldTask;
	}

	private static void ClearIfCompleted<T>(ref SyncTask<T> oldTask, Func<SyncTask<T>> taskProvider)
	{
		SyncTask<T> obj = oldTask;
		if (obj == null || obj.GetAwaiter().IsCompleted)
		{
			if (oldTask != null)
			{
				SyncTask<T> obj2 = oldTask;
				oldTask = null;
				obj2.GetAwaiter().GetResult();
			}
			oldTask = taskProvider();
		}
	}

	public static NextFrameTask NextFrame()
	{
		return new NextFrameTask();
	}
}
