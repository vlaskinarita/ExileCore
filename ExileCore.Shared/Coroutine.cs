using System;
using System.Collections;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;

namespace ExileCore.Shared;

public class Coroutine
{
	private IEnumerator _enumerator;

	public bool IsDone { get; private set; }

	public string Name { get; set; }

	public IPlugin Owner { get; private set; }

	public string OwnerName { get; }

	public bool Running { get; private set; }

	public bool AutoResume { get; set; } = true;


	public string TimeoutForAction { get; private set; }

	public long Ticks { get; private set; } = -1L;


	public CoroutinePriority Priority { get; set; }

	public DateTime Started { get; set; }

	public Action Action { get; private set; }

	public IYieldBase Condition { get; private set; }

	public bool ThisIsSimple => Action != null;

	public bool NextIterRealWork { get; set; }

	public bool SyncModWork { get; set; }

	public event Action OnAutoRestart;

	public event EventHandler WhenDone;

	private Coroutine(string name, IPlugin owner)
	{
		Name = name ?? MathHepler.GetRandomWord(13);
		Owner = owner;
		OwnerName = ((Owner == null) ? "Free" : Owner.GetType().Namespace);
	}

	public Coroutine(Action action, IYieldBase condition, IPlugin owner, string name = null, bool infinity = true, bool autoStart = true)
		: this(name, owner)
	{
		Running = autoStart;
		Started = DateTime.Now;
		TimeoutForAction = ((condition is WaitTime waitTime) ? waitTime.Milliseconds.ToString() : ((condition is WaitRender waitRender) ? waitRender.HowManyRenderCountWait.ToString() : ((condition is WaitRandom waitRandom) ? waitRandom.Timeout : ((!(condition is WaitFunction)) ? TimeoutForAction : "Function -1"))));
		Action = action;
		Condition = condition;
		if (infinity)
		{
			_enumerator = CoroutineAction(action);
		}
		else
		{
			_enumerator = CoroutineAction(action);
		}
		IEnumerator CoroutineAction(Action a)
		{
			yield return YieldBase.RealWork;
			while (true)
			{
				try
				{
					a?.Invoke();
				}
				catch (Exception value)
				{
					Console.WriteLine($"Coroutine {Name} in {OwnerName} error -> {value}");
				}
				Ticks++;
				yield return Condition.GetEnumerator();
			}
		}
		IEnumerator CoroutineAction(Action a)
		{
			yield return Condition.GetEnumerator();
			a?.Invoke();
			Ticks++;
		}
	}

	public Coroutine(Action action, int waitMilliseconds, IPlugin owner, string name = null, bool autoStart = true)
		: this(action, new WaitTime(waitMilliseconds), owner, name, autoStart)
	{
	}

	public Coroutine(IEnumerator enumerator, IPlugin owner, string name = null, bool autoStart = true)
		: this(name, owner)
	{
		Running = autoStart;
		Started = DateTime.Now;
		TimeoutForAction = "Not simple -1";
		_enumerator = enumerator;
	}

	public void UpdateCondtion(IYieldBase condition)
	{
		string timeoutForAction = ((condition is WaitTime waitTime) ? waitTime.Milliseconds.ToString() : ((condition is WaitRender waitRender) ? waitRender.HowManyRenderCountWait.ToString() : ((!(condition is WaitFunction)) ? TimeoutForAction : "Function")));
		TimeoutForAction = timeoutForAction;
		Condition = condition;
	}

	public IEnumerator GetEnumerator()
	{
		return _enumerator;
	}

	public void UpdateTicks(uint tick)
	{
		Ticks = tick;
	}

	public void Resume()
	{
		Running = true;
	}

	public void AutoRestart()
	{
		this.OnAutoRestart?.Invoke();
	}

	public void Pause(bool force = false)
	{
		if (Priority != CoroutinePriority.Critical || force)
		{
			Running = false;
		}
	}

	public bool Done(bool force = false)
	{
		if (Priority == CoroutinePriority.Critical && !force)
		{
			return false;
		}
		Running = false;
		IsDone = true;
		this.WhenDone?.Invoke(this, EventArgs.Empty);
		return IsDone;
	}

	public void UpdateAction(Action action)
	{
		if (Action != null)
		{
			Action = action;
		}
	}

	public void UpdateAction(IEnumerator action)
	{
		if (_enumerator != null)
		{
			_enumerator = action;
		}
	}

	public bool MoveNext()
	{
		return MoveNext(_enumerator);
	}

	private bool MoveNext(IEnumerator enumerator)
	{
		if (IsDone)
		{
			return false;
		}
		bool result;
		if (enumerator.Current is IEnumerator enumerator2 && MoveNext(enumerator2))
		{
			result = true;
		}
		else
		{
			result = enumerator.MoveNext();
			NextIterRealWork = enumerator.Current == YieldBase.RealWork;
		}
		return result;
	}
}
