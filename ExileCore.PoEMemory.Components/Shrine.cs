namespace ExileCore.PoEMemory.Components;

public class Shrine : Component
{
	public bool IsAvailable
	{
		get
		{
			if (base.Address != 0L)
			{
				return base.M.Read<byte>(base.Address + 36) == 0;
			}
			return false;
		}
	}
}
