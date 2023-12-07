namespace ExileCore.PoEMemory.Components;

public class Charges : Component
{
	public int NumCharges
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 24);
		}
	}

	public int ChargesPerUse
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 16, new int[1] { 24 });
		}
	}

	public int ChargesMax
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 16, new int[1] { 20 });
		}
	}
}
