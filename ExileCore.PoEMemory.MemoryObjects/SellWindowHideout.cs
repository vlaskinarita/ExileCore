namespace ExileCore.PoEMemory.MemoryObjects;

public class SellWindowHideout : SellWindow
{
	public override Element YourOffer => SellDialog?.GetChildAtIndex(1);

	public override Element OtherOffer => SellDialog?.GetChildAtIndex(0);

	public override Element SellDialog => GetChildAtIndex(4);
}
