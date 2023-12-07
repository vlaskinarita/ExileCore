using System.Collections;
using System.Diagnostics;

namespace ExileCore.Shared;

public abstract class YieldBase : IYieldBase, IEnumerable, IEnumerator
{
	protected static readonly Stopwatch sw = Stopwatch.StartNew();

	public static object RealWork { get; } = new object();


	public object Current { get; protected set; }

	public bool MoveNext()
	{
		if (((IEnumerator)Current).MoveNext())
		{
			return true;
		}
		Current = GetEnumerator();
		return false;
	}

	public void Reset()
	{
	}

	public abstract IEnumerator GetEnumerator();
}
