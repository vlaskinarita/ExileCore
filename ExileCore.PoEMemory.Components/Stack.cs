namespace ExileCore.PoEMemory.Components;

public class Stack : Component
{
	public int Size
	{
		get
		{
			if (base.Address != 0L)
			{
				return base.M.Read<int>(base.Address + 24);
			}
			return 0;
		}
	}

	public CurrencyInfo Info
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			return ReadObject<CurrencyInfo>(base.Address + 16);
		}
	}

	public bool FullStack => Info.MaxStackSize == Size;
}
