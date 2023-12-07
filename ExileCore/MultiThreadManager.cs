using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using JM.LinqFaster;

namespace ExileCore;

public class MultiThreadManager
{
	private const long CriticalWorkTimeMs = 750L;

	private readonly object locker = new object();

	private int _lock;

	private object _objectInitWork;

	private readonly List<ThreadUnit> BrokenThreads = new List<ThreadUnit>();

	private readonly ConcurrentQueue<ThreadUnit> FreeThreads = new ConcurrentQueue<ThreadUnit>();

	private readonly ConcurrentQueue<Job> Jobs = new ConcurrentQueue<Job>();

	private readonly Queue<Job> processJobs = new Queue<Job>();

	private volatile bool ProcessWorking;

	private SpinWait spinWait;

	private ThreadUnit[] threads;

	public int FailedThreadsCount { get; private set; }

	public int ThreadsCount { get; private set; }

	public MultiThreadManager(int countThreads)
	{
		spinWait = default(SpinWait);
		ChangeNumberThreads(countThreads);
	}

	public void ChangeNumberThreads(int countThreads)
	{
		lock (locker)
		{
			if (countThreads == ThreadsCount)
			{
				return;
			}
			ThreadsCount = countThreads;
			if (threads != null)
			{
				ThreadUnit[] array = threads;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Abort();
				}
				while (!FreeThreads.IsEmpty)
				{
					FreeThreads.TryDequeue(out var _);
				}
			}
			if (countThreads > 0)
			{
				threads = new ThreadUnit[ThreadsCount];
				for (int j = 0; j < ThreadsCount; j++)
				{
					threads[j] = new ThreadUnit($"Thread #{j}", j);
					FreeThreads.Enqueue(threads[j]);
				}
			}
			else
			{
				threads = null;
			}
		}
	}

	public Job AddJob(Job job)
	{
		job.IsStarted = true;
		bool flag = false;
		if (!FreeThreads.IsEmpty)
		{
			FreeThreads.TryDequeue(out var result);
			if (result != null)
			{
				flag = result.AddJob(job);
				if (result.Free)
				{
					FreeThreads.Enqueue(result);
				}
			}
		}
		if (!flag)
		{
			Jobs.Enqueue(job);
		}
		return job;
	}

	public Job AddJob(Action action, string name)
	{
		Job job = new Job(name, action);
		return AddJob(job);
	}

	public void Process(object o)
	{
		if (threads == null || Interlocked.CompareExchange(ref _lock, 1, 0) == 1)
		{
			return;
		}
		if (ProcessWorking)
		{
			DebugWindow.LogMsg($"WTF {_objectInitWork.GetType()}");
		}
		_objectInitWork = o;
		ProcessWorking = true;
		spinWait.Reset();
		Job result;
		while (Jobs.TryDequeue(out result))
		{
			processJobs.Enqueue(result);
		}
		if (ThreadsCount > 1)
		{
			while (processJobs.Count > 0)
			{
				if (!FreeThreads.IsEmpty)
				{
					FreeThreads.TryDequeue(out var result2);
					Job job = processJobs.Dequeue();
					if (!result2.AddJob(job))
					{
						processJobs.Enqueue(job);
					}
					else if (result2.Free)
					{
						FreeThreads.Enqueue(result2);
					}
					continue;
				}
				spinWait.SpinOnce();
				bool flag = true;
				for (int i = 0; i < threads.Length; i++)
				{
					ThreadUnit threadUnit = threads[i];
					if (threadUnit.Free)
					{
						flag = false;
						FreeThreads.Enqueue(threadUnit);
					}
				}
				if (!flag)
				{
					continue;
				}
				for (int j = 0; j < threads.Length; j++)
				{
					ThreadUnit threadUnit2 = threads[j];
					long workingTime = threadUnit2.WorkingTime;
					if (workingTime > 750)
					{
						DebugWindow.LogMsg($"Repair thread #{threadUnit2.Number} with Job1: {threadUnit2.Job.Name} (C: {threadUnit2.Job.IsCompleted} F: {threadUnit2.Job.IsFailed}) && Job2:{threadUnit2.SecondJob.Name} (C: {threadUnit2.SecondJob.IsCompleted} F: {threadUnit2.SecondJob.IsFailed}) Time: {workingTime} > {workingTime >= 750}", 5f);
						threadUnit2.Abort();
						BrokenThreads.Add(threadUnit2);
						ThreadUnit threadUnit3 = new ThreadUnit($"Repair critical time {threadUnit2.Number}", threadUnit2.Number);
						threads[threadUnit2.Number] = threadUnit3;
						FreeThreads.Enqueue(threadUnit3);
						Thread.Sleep(5);
						FailedThreadsCount++;
						break;
					}
				}
			}
		}
		else
		{
			ThreadUnit threadUnit4 = threads[0];
			while (processJobs.Count > 0)
			{
				if (threadUnit4.Free)
				{
					Job job2 = processJobs.Dequeue();
					threadUnit4.AddJob(job2);
					continue;
				}
				spinWait.SpinOnce();
				if (threadUnit4.WorkingTime > 750)
				{
					LogThreadOvertime(threadUnit4);
					threadUnit4.Abort();
					BrokenThreads.Add(threadUnit4);
					threadUnit4 = new ThreadUnit($"Repair critical time {threadUnit4.Number}", threadUnit4.Number);
					Thread.Sleep(5);
					FailedThreadsCount++;
				}
			}
		}
		if (BrokenThreads.Count > 0)
		{
			long num = 1500L;
			for (int k = 0; k < BrokenThreads.Count; k++)
			{
				ThreadUnit threadUnit5 = BrokenThreads[k];
				if (threadUnit5 != null && threadUnit5.WorkingTime > num)
				{
					LogThreadOvertime(threadUnit5);
					DebugWindow.LogError("Thread does not respond to stop requests. Usually this indicates a broken plugin", 10f);
					BrokenThreads[k] = null;
				}
			}
			if (BrokenThreads.AllF((ThreadUnit x) => x == null))
			{
				BrokenThreads.Clear();
			}
		}
		Interlocked.CompareExchange(ref _lock, 0, 1);
		ProcessWorking = false;
	}

	private static void LogThreadOvertime(ThreadUnit threadUnit)
	{
		DebugWindow.LogMsg($"Repair thread #{threadUnit.Number} with Unit Job1: {threadUnit.Job.Name} (C: {threadUnit.Job.IsCompleted} F: {threadUnit.Job.IsFailed}) && Job2:{threadUnit.SecondJob.Name} (C: {threadUnit.SecondJob.IsCompleted} F: {threadUnit.SecondJob.IsFailed}) Time: {threadUnit.WorkingTime} > {750}", 5f);
	}

	public void Close()
	{
		ThreadUnit[] array = threads;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Abort();
		}
	}
}
