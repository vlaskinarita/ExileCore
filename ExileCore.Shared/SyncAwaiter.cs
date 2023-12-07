using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ExileCore.Shared;

public class SyncAwaiter<T> : SyncAwaiter
{
	public bool IsCompleted => ResultTask.Task.IsCompleted;

	internal TaskCompletionSource<T> ResultTask { get; } = new TaskCompletionSource<T>();


	public T GetResult()
	{
		return ResultTask.Task.GetAwaiter().GetResult();
	}

	public override void OnCompleted(Action completion)
	{
		ResultTask.Task.ContinueWith(delegate
		{
			completion();
		}, TaskContinuationOptions.ExecuteSynchronously);
	}
}
public abstract class SyncAwaiter : INotifyCompletion
{
	private record Unsubscribe(SyncAwaiter Parent, SyncAwaiter Child) : IDisposable
	{
		public void Dispose()
		{
			Parent._childAwaiters.TryRemove(Child, out var _);
		}
	}

	private readonly Queue<Action> _methodExecutionQueue = new Queue<Action>();

	private readonly ConcurrentDictionary<SyncAwaiter, bool> _childAwaiters = new ConcurrentDictionary<SyncAwaiter, bool>();

	public abstract void OnCompleted(Action completion);

	internal IDisposable RedirectExecutionQueue(SyncAwaiter target)
	{
		target._childAwaiters.TryAdd(this, value: true);
		Unsubscribe unsubscribe = new Unsubscribe(target, this);
		OnCompleted(unsubscribe.Dispose);
		return unsubscribe;
	}

	internal void EnqueueItem(Action item)
	{
		_methodExecutionQueue.Enqueue(item);
	}

	public bool PumpEvents()
	{
		bool flag = false;
		try
		{
			bool flag2;
			do
			{
				flag2 = false;
				Action result;
				while (_methodExecutionQueue.TryDequeue(out result))
				{
					flag = (flag2 = true);
					result?.Invoke();
				}
				foreach (SyncAwaiter key in _childAwaiters.Keys)
				{
					flag |= (flag2 |= key.PumpEvents());
				}
			}
			while (flag2);
		}
		catch
		{
		}
		return flag;
	}
}
