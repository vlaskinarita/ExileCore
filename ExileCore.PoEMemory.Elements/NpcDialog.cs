using System.Collections.Generic;

namespace ExileCore.PoEMemory.Elements;

public class NpcDialog : Element
{
	public string NpcName => GetChildAtIndex(1)?.GetChildAtIndex(3)?.Text;

	public Element NpcLineWrapper => GetChildAtIndex(0)?.GetChildAtIndex(2);

	public List<NpcLine> NpcLines => GetNpcLines();

	public bool IsLoreTalkVisible
	{
		get
		{
			if (NpcLines.Count == 0)
			{
				return base.IsVisible;
			}
			return false;
		}
	}

	private List<NpcLine> GetNpcLines()
	{
		List<NpcLine> list = new List<NpcLine>();
		if (NpcLineWrapper?.Children == null)
		{
			DebugWindow.LogError("NpcLineWrapper?.Children is null, check offsets");
			return list;
		}
		foreach (Element item2 in NpcLineWrapper?.Children)
		{
			try
			{
				NpcLine item = new NpcLine(item2);
				list.Add(item);
			}
			catch
			{
			}
		}
		return list;
	}
}
