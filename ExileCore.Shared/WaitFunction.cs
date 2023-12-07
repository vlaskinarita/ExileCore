using System;
using System.Collections;

namespace ExileCore.Shared;

[Obsolete("Use WaitFunctionTimed instead to prevent unwanted endless loops")]
public class WaitFunction : YieldBase
{
	private readonly Func<bool> fn;

	public WaitFunction(Func<bool> fn)
	{
		this.fn = fn;
		base.Current = GetEnumerator();
	}

	public sealed override IEnumerator GetEnumerator()
	{
		while (!fn())
		{
			yield return null;
		}
		yield return YieldBase.RealWork;
	}
}
