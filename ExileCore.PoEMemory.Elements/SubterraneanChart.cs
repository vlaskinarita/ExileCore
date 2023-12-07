namespace ExileCore.PoEMemory.Elements;

public class SubterraneanChart : Element
{
	private DelveElement _grid;

	public DelveElement GridElement
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return _grid ?? (_grid = GetObject<DelveElement>(base.M.Read<long>(base.Address + 520, new int[1] { 1840 })));
		}
	}
}
