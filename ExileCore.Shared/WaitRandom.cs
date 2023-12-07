using System;
using System.Collections;

namespace ExileCore.Shared;

public class WaitRandom : YieldBase
{
	private static readonly Random rnd = new Random();

	private readonly int _maxWait;

	private readonly int _minWait;

	public string Timeout => _minWait + "-" + _maxWait;

	public WaitRandom(int minWait, int maxWait)
	{
		_minWait = minWait;
		_maxWait = maxWait;
		base.Current = GetEnumerator();
	}

	public sealed override IEnumerator GetEnumerator()
	{
		long wait = YieldBase.sw.ElapsedMilliseconds + rnd.Next(_minWait, _maxWait);
		while (YieldBase.sw.ElapsedMilliseconds < wait)
		{
			yield return null;
		}
		yield return YieldBase.RealWork;
	}
}
