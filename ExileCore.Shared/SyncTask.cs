using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ExileCore.Shared;

[AsyncMethodBuilder(typeof(SyncTaskMethodBuilder<>))]
public class SyncTask<T>
{
	internal SyncAwaiter<T> Awaiter { get; } = new SyncAwaiter<T>();


	public SyncAwaiter<T> GetAwaiter()
	{
		return Awaiter;
	}
}
public static class SyncTask
{
	public static SyncTask<SyncTask<T>> WhenAny<T>(params SyncTask<T>[] tasks)
	{
		SyncTask<SyncTask<T>> aggregateTask = new SyncTask<SyncTask<T>>();
		SyncTask<T> syncTask = tasks.FirstOrDefault((SyncTask<T> x) => x.Awaiter.IsCompleted);
		if (syncTask != null)
		{
			aggregateTask.GetAwaiter().ResultTask.SetResult(syncTask);
			return aggregateTask;
		}
		List<IDisposable> disposeList = new List<IDisposable>();
		SyncTask<T>[] array = tasks;
		foreach (SyncTask<T> syncTask2 in array)
		{
			disposeList.Add(syncTask2.Awaiter.RedirectExecutionQueue(aggregateTask.Awaiter));
		}
		array = tasks;
		foreach (SyncTask<T> childTask in array)
		{
			childTask.Awaiter.OnCompleted(delegate
			{
				if (aggregateTask.GetAwaiter().ResultTask.TrySetResult(childTask))
				{
					foreach (IDisposable item in disposeList)
					{
						item.Dispose();
					}
				}
			});
		}
		return aggregateTask;
	}
}
