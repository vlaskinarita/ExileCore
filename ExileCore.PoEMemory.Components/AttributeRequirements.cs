namespace ExileCore.PoEMemory.Components;

public class AttributeRequirements : Component
{
	public int strength
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

	public int dexterity
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

	public int intelligence
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
