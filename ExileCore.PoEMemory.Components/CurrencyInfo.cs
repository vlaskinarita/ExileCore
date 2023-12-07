namespace ExileCore.PoEMemory.Components;

public class CurrencyInfo : Component
{
	public int MaxStackSize
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 40);
		}
	}
}
