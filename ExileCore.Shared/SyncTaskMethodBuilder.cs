using System;
using System.Runtime.CompilerServices;

namespace ExileCore.Shared;

public class SyncTaskMethodBuilder<T>
{
	private IAsyncStateMachine _stateMachine;

	public SyncTask<T> Task { get; } = new SyncTask<T>();


	public static SyncTaskMethodBuilder<T> Create()
	{
		return new SyncTaskMethodBuilder<T>();
	}

	public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
	{
		_stateMachine = stateMachine;
		_stateMachine.SetStateMachine(stateMachine);
		Task.Awaiter.EnqueueItem(_stateMachine.MoveNext);
	}

	public void SetStateMachine(IAsyncStateMachine stateMachine)
	{
		_stateMachine = stateMachine;
	}

	public void SetException(Exception exception)
	{
		Task.Awaiter.ResultTask.SetException(exception);
	}

	public void SetResult(T result)
	{
		Task.Awaiter.ResultTask.SetResult(result);
	}

	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		_stateMachine = stateMachine;
		_stateMachine.SetStateMachine(stateMachine);
		ref TAwaiter reference = ref awaiter;
		TAwaiter val = default(TAwaiter);
		if (val == null)
		{
			val = reference;
			reference = ref val;
		}
		reference.OnCompleted(delegate
		{
			Task.Awaiter.EnqueueItem(_stateMachine.MoveNext);
		});
		if ((object)awaiter is SyncAwaiter syncAwaiter)
		{
			IDisposable disposable = syncAwaiter.RedirectExecutionQueue(Task.Awaiter);
			syncAwaiter.OnCompleted(disposable.Dispose);
		}
	}

	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
	{
		AwaitOnCompleted(ref awaiter, ref stateMachine);
	}
}
