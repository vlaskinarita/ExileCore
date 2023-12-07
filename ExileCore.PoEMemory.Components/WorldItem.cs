using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;

namespace ExileCore.PoEMemory.Components;

public class WorldItem : Component
{
	private readonly CachedValue<Entity> _cachedValue;

	public Entity ItemEntity => _cachedValue.Value;

	public uint AllocatedToPlayer => base.M.Read<uint>(base.Address + 48);

	public int AllocatedToOtherTime => base.M.Read<int>(base.Address + 52);

	public DateTime DroppedTime
	{
		get
		{
			int num = base.M.Read<int>(base.Address + 56);
			return DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount) + TimeSpan.FromMilliseconds(num);
		}
	}

	public bool IsPermanentlyAllocated => AllocatedToOtherTime == 300000;

	public DateTime PublicTime => DroppedTime + TimeSpan.FromMilliseconds(AllocatedToOtherTime);

	public bool AllocatedToSomeoneElse
	{
		get
		{
			if (AllocatedToPlayer != 0)
			{
				return Entity.Player.GetComponent<Player>().AllocatedLootId != AllocatedToPlayer;
			}
			return false;
		}
	}

	public WorldItem()
	{
		_cachedValue = new FrameCache<Entity>(() => (base.Address == 0L) ? null : ReadObject<Entity>(base.Address + 40));
	}
}
