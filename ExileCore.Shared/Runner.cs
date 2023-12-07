using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ExileCore.Shared.Interfaces;
using JM.LinqFaster;

namespace ExileCore.Shared;

public class Runner
{
	private readonly HashSet<Coroutine> _autorestartCoroutines = new HashSet<Coroutine>();

	private readonly List<CoroutineDetails> _finishedCoroutines = new List<CoroutineDetails>();

	private readonly object locker = new object();

	private readonly Stopwatch sw;

	private readonly List<Job> jobs = new List<Job>(16);

	private double time;

	public MultiThreadManager MultiThreadManager { get; set; }

	public string Name { get; }

	public int CriticalTimeWork { get; set; } = 150;


	public bool IsRunning => Coroutines.Count > 0;

	public int CoroutinesCount => Coroutines.Count;

	public List<CoroutineDetails> FinishedCoroutines => _finishedCoroutines.ToList();

	public int FinishedCoroutineCount { get; private set; }

	public IList<Coroutine> Coroutines { get; } = new List<Coroutine>();


	public IEnumerable<Coroutine> WorkingCoroutines => Coroutines.Where((Coroutine x) => x.Running);

	public int CountAddCoroutines { get; private set; }

	public int CountFalseAddCoroutines { get; private set; }

	public int IterationPerFrame { get; set; } = 2;


	public Dictionary<string, double> CoroutinePerformance { get; } = new Dictionary<string, double>();


	public int Count => Coroutines.Count;

	public Runner(string name)
	{
		Name = name;
		sw = Stopwatch.StartNew();
	}

	public Coroutine Run(IEnumerator enumerator, IPlugin owner, string name = null)
	{
		if (enumerator == null)
		{
			throw new NullReferenceException("Coroutine cant not be null.");
		}
		Coroutine routine = new Coroutine(enumerator, owner, name);
		lock (locker)
		{
			Coroutine coroutine = Coroutines.FirstOrDefault((Coroutine x) => x.Name == routine.Name && x.Owner == routine.Owner);
			if (coroutine != null)
			{
				CountFalseAddCoroutines++;
				return coroutine;
			}
			Coroutines.Add(routine);
			CoroutinePerformance.TryGetValue(routine.Name, out var value);
			CoroutinePerformance[routine.Name] = value;
			CountAddCoroutines++;
			return routine;
		}
	}

	public Coroutine Run(Coroutine routine)
	{
		if (routine == null)
		{
			throw new NullReferenceException("Coroutine cant not be null.");
		}
		lock (locker)
		{
			Coroutine coroutine = Coroutines.FirstOrDefault((Coroutine x) => x.Name == routine.Name && x.Owner == routine.Owner);
			if (coroutine != null)
			{
				CountFalseAddCoroutines++;
				return coroutine;
			}
			Coroutines.Add(routine);
			CoroutinePerformance.TryGetValue(routine.Name, out var value);
			CoroutinePerformance[routine.Name] = value;
			CountAddCoroutines++;
			return routine;
		}
	}

	public void PauseCoroutines(IList<Coroutine> coroutines)
	{
		foreach (Coroutine coroutine in coroutines)
		{
			coroutine.Pause();
		}
	}

	public void ResumeCoroutines(IList<Coroutine> coroutines)
	{
		foreach (Coroutine coroutine in coroutines)
		{
			if (coroutine.AutoResume)
			{
				coroutine.Resume();
			}
		}
	}

	public Coroutine FindByName(string name)
	{
		return Coroutines.FirstOrDefault((Coroutine x) => x.Name == name);
	}

	public Coroutine ByFuncName(Func<string, bool> predicate)
	{
		return Coroutines.FirstOrDefault((Coroutine x) => predicate(x.Name));
	}

	public void Update()
	{
		if (Coroutines.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < Coroutines.Count; i++)
		{
			Coroutine coroutine = Coroutines[i];
			if (!coroutine.IsDone)
			{
				if (!coroutine.Running)
				{
					continue;
				}
				try
				{
					time = sw.Elapsed.TotalMilliseconds;
					double num = 0.0;
					if (!coroutine.MoveNext())
					{
						coroutine.Done();
					}
					num = sw.Elapsed.TotalMilliseconds - time;
					CoroutinePerformance[coroutine.Name] += num;
					if (num > (double)CriticalTimeWork)
					{
						Console.WriteLine($"Coroutine {coroutine.Name} ({coroutine.OwnerName}) [{Name}] {num} $Performance coroutine");
					}
				}
				catch (Exception value)
				{
					CoroutinePerformance[$"{coroutine.Name} | ({DateTime.Now})"] = CoroutinePerformance[coroutine.Name] + (sw.Elapsed.TotalMilliseconds - time);
					CoroutinePerformance[coroutine.Name] = 0.0;
					Console.WriteLine($"Coroutine {coroutine.Name} ({coroutine.OwnerName}) error: {value}");
				}
			}
			else
			{
				_finishedCoroutines.Add(new CoroutineDetails(coroutine.Name, coroutine.OwnerName, coroutine.Ticks, coroutine.Started, DateTime.Now));
				FinishedCoroutineCount++;
				Coroutines.Remove(coroutine);
			}
		}
	}

	public void ParallelUpdate()
	{
		if (MultiThreadManager == null || MultiThreadManager.ThreadsCount < 1)
		{
			Update();
		}
		else
		{
			if (Coroutines.Count <= 0)
			{
				return;
			}
			jobs.Clear();
			for (int i = 0; i < Coroutines.Count; i++)
			{
				Coroutine coroutine = Coroutines[i];
				if (!coroutine.IsDone)
				{
					if (!coroutine.Running)
					{
						continue;
					}
					if (coroutine.NextIterRealWork && !coroutine.SyncModWork)
					{
						Job item = MultiThreadManager.AddJob(delegate
						{
							if (!coroutine.MoveNext())
							{
								coroutine.Done();
							}
						}, coroutine.Name);
						jobs.Add(item);
						continue;
					}
					time = sw.Elapsed.TotalMilliseconds;
					double num = 0.0;
					if (!coroutine.MoveNext())
					{
						coroutine.Done();
					}
					num = sw.Elapsed.TotalMilliseconds - time;
					CoroutinePerformance[coroutine.Name] += num;
					if (num > (double)CriticalTimeWork)
					{
						Console.WriteLine($"Coroutine {coroutine.Name} ({coroutine.OwnerName}) [{Name}] {num} $Performance coroutine");
					}
				}
				else
				{
					_finishedCoroutines.Add(new CoroutineDetails(coroutine.Name, coroutine.OwnerName, coroutine.Ticks, coroutine.Started, DateTime.Now));
					FinishedCoroutineCount++;
					Coroutines.Remove(coroutine);
				}
			}
			MultiThreadManager.Process(this);
			SpinWait.SpinUntil(() => jobs.AllF((Job job) => job.IsCompleted), 500);
			foreach (Job job in jobs)
			{
				CoroutinePerformance[job.Name] += job.ElapsedMs;
			}
		}
	}
}
