namespace ExileCore.PoEMemory.Elements;

public class SearchBarElement : Element
{
	public Element SearchBar => GetChildAtIndex(0);

	public Element ClearSearchBarButton => GetChildAtIndex(1);
}
