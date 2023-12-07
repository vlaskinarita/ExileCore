namespace ExileCore.PoEMemory.Elements;

public class SentinelPanel : Element
{
	public SentinelSubPanel RedSentinelSubPanel => GetChildAtIndex(0)?.AsObject<SentinelSubPanel>();

	public SentinelSubPanel BlueSentinelSubPanel => GetChildAtIndex(1)?.AsObject<SentinelSubPanel>();

	public SentinelSubPanel YellowSentinelSubPanel => GetChildAtIndex(2)?.AsObject<SentinelSubPanel>();
}
