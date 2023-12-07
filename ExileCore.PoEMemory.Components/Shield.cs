namespace ExileCore.PoEMemory.Components;

public class Shield : Component
{
	public int ChanceToBlock
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
}
