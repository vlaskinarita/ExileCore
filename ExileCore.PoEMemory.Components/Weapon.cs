namespace ExileCore.PoEMemory.Components;

public class Weapon : Component
{
	public int WeaponType
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 32, new int[1]);
		}
	}

	public int DamageMin
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 32, new int[1] { 4 });
		}
	}

	public int DamageMax
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 32, new int[1] { 8 });
		}
	}

	public int AttackTime
	{
		get
		{
			if (base.Address == 0L)
			{
				return 1;
			}
			return base.M.Read<int>(base.Address + 32, new int[1] { 12 });
		}
	}

	public int CritChance
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 32, new int[1] { 16 });
		}
	}

	public int WeaponRange
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 32, new int[1] { 24 });
		}
	}
}
