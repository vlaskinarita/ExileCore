using SharpDX;

namespace ExileCore.PoEMemory.Components;

public class TriggerableBlockage : Component
{
	public bool IsClosed
	{
		get
		{
			if (base.Address != 0L)
			{
				return base.M.Read<byte>(base.Address + 48) == 1;
			}
			return false;
		}
	}

	public bool IsOpened
	{
		get
		{
			if (base.Address != 0L)
			{
				return base.M.Read<byte>(base.Address + 48) == 0;
			}
			return false;
		}
	}

	public Point Min => new Point(base.M.Read<int>(base.Address + 80), base.M.Read<int>(base.Address + 84));

	public Point Max => new Point(base.M.Read<int>(base.Address + 88), base.M.Read<int>(base.Address + 92));

	public byte[] Data
	{
		get
		{
			long num = base.M.Read<long>(base.Address + 56);
			long num2 = base.M.Read<long>(base.Address + 64);
			return base.M.ReadBytes(num, (int)(num2 - num));
		}
	}
}
