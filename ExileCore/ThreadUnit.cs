using System;
using System.Diagnostics;
using System.Threading;

namespace ExileCore;

public class ThreadUnit
{
	private readonly AutoResetEvent _event;

	private readonly Stopwatch sw;

	private readonly Thread thread;

	private bool _wait = true;

	private bool running = true;

	private Job _job;

	private Job _secondJob;

	public static int CountJobs { get; set; }

	public static int CountWait { get; set; }

	public int Number { get; }

	public Job Job
	{
		get
		{
			return _job;
		}
		private set
		{
			_job = value;
		}
	}

	public Job SecondJob
	{
		get
		{
			return _secondJob;
		}
		private set
		{
			_secondJob = value;
		}
	}

	public bool Free
	{
		get
		{
			if (!Job.IsCompleted)
			{
				return SecondJob.IsCompleted;
			}
			return true;
		}
	}

	public long WorkingTime => sw.ElapsedMilliseconds;

	public ThreadUnit(string name, int number)
	{
		Number = number;
		Job = CreateInitJob();
		SecondJob = CreateInitJob();
		_event = new AutoResetEvent(initialState: false);
		thread = new Thread(DoWork)
		{
			Name = name,
			IsBackground = true
		};
		thread.Start();
		sw = Stopwatch.StartNew();
	}

	private static Job CreateInitJob()
	{
		return new Job("InitJob", null)
		{
			IsCompleted = true
		};
	}

	private void DoWork()
	{
		while (running)
		{
			Job job = Job;
			Job secondJob = SecondJob;
			if (job.IsCompleted && secondJob.IsCompleted && Interlocked.CompareExchange(ref _job, CreateInitJob(), job) == job && Interlocked.CompareExchange(ref _secondJob, CreateInitJob(), secondJob) == secondJob)
			{
				_event.WaitOne();
				CountWait++;
				_wait = true;
			}
			if (!Job.IsCompleted)
			{
				try
				{
					sw.Restart();
					Job.Work?.Invoke();
				}
				catch (Exception ex)
				{
					DebugWindow.LogError(ex.ToString());
					Job.IsFailed = true;
				}
				finally
				{
					Job.ElapsedMs = sw.Elapsed.TotalMilliseconds;
					Job.IsCompleted = true;
					sw.Restart();
				}
			}
			if (SecondJob.IsCompleted)
			{
				continue;
			}
			try
			{
				sw.Restart();
				SecondJob.Work?.Invoke();
			}
			catch (Exception ex2)
			{
				DebugWindow.LogError(ex2.ToString());
				SecondJob.IsFailed = true;
			}
			finally
			{
				SecondJob.ElapsedMs = sw.Elapsed.TotalMilliseconds;
				SecondJob.IsCompleted = true;
				sw.Restart();
			}
		}
	}

	public bool AddJob(Job job)
	{
		job.WorkingOnThread = this;
		bool flag = false;
		if (Job.IsCompleted)
		{
			Job = job;
			flag = true;
			CountJobs++;
		}
		else if (SecondJob.IsCompleted)
		{
			SecondJob = job;
			flag = true;
			CountJobs++;
		}
		if (_wait && flag)
		{
			_wait = false;
			_event.Set();
		}
		return flag;
	}

	public void Abort()
	{
		Job.IsCompleted = true;
		SecondJob.IsCompleted = true;
		Job.IsFailed = true;
		Job.IsFailed = true;
		if (_wait)
		{
			_event.Set();
		}
		running = false;
	}
}
