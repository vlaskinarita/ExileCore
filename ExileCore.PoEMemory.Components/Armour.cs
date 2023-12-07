namespace ExileCore.PoEMemory.Components;

public class Armour : Component
{
	public int EvasionScore
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 16, new int[1] { 16 });
		}
	}

	public int ArmourScore
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

	public int EnergyShieldScore
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
}
