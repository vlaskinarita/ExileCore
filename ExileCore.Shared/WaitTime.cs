using System.Collections;

namespace ExileCore.Shared;

public class WaitTime : YieldBase
{
	public int Milliseconds { get; }

	public WaitTime(int milliseconds)
	{
		Milliseconds = milliseconds;
		base.Current = GetEnumerator();
	}

	public sealed override IEnumerator GetEnumerator()
	{
		double wait = YieldBase.sw.Elapsed.TotalMilliseconds + (double)Milliseconds;
		while (YieldBase.sw.Elapsed.TotalMilliseconds < wait)
		{
			yield return null;
		}
		yield return YieldBase.RealWork;
	}
}
