namespace ExileCore.PoEMemory.Components;

public class Quality : Component
{
	public int ItemQuality
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
}
