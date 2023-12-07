namespace ExileCore.PoEMemory.Elements;

public class StashTabElement : Element
{
	public Element StashTab
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return ReadObjectAt<Element>(448);
		}
	}

	public string TabName => StashTab?.Text ?? string.Empty;
}
