using System;
using System.Collections;

namespace ExileCore.Shared;

public class WaitFunctionTimed : YieldBase
{
	private readonly Func<bool> fn;

	public int Milliseconds { get; }

	public bool StopCode { get; }

	public string ErrorMessage { get; }

	public WaitFunctionTimed(Func<bool> fn, bool stopCode = false, int maxWait = 1000, string errorMessage = "")
	{
		this.fn = fn;
		Milliseconds = maxWait;
		StopCode = stopCode;
		ErrorMessage = errorMessage;
		base.Current = GetEnumerator();
	}

	public sealed override IEnumerator GetEnumerator()
	{
		double wait = YieldBase.sw.Elapsed.TotalMilliseconds + (double)Milliseconds;
		while (!fn() && YieldBase.sw.Elapsed.TotalMilliseconds < wait)
		{
			yield return null;
		}
		if (!fn() && StopCode)
		{
			if (ErrorMessage != "")
			{
				DebugWindow.LogMsg(ErrorMessage);
			}
			Logger.Log.Error($"Code Stopped in {this}");
		}
		else
		{
			yield return YieldBase.RealWork;
		}
	}
}
