using System.Collections;

namespace ExileCore.Shared;

public class WaitRender : YieldBase
{
	public static int FrameCount { get; private set; }

	public long HowManyRenderCountWait { get; }

	public WaitRender(long howManyRenderCountWait = 1L)
	{
		HowManyRenderCountWait = howManyRenderCountWait;
		base.Current = GetEnumerator();
	}

	public static void Frame()
	{
		FrameCount++;
	}

	public sealed override IEnumerator GetEnumerator()
	{
		long wait = FrameCount + HowManyRenderCountWait;
		while (FrameCount < wait)
		{
			yield return null;
		}
		yield return YieldBase.RealWork;
	}
}
