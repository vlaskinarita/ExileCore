using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class Chest : Component
{
	private readonly CachedValue<ChestComponentOffsets> _cachedValue;

	private readonly CachedValue<StrongboxChestComponentData> _cachedValueStrongboxData;

	public bool IsOpened
	{
		get
		{
			if (base.Address != 0L)
			{
				return _cachedValue.Value.IsOpened;
			}
			return false;
		}
	}

	public bool IsLocked
	{
		get
		{
			if (base.Address != 0L)
			{
				return _cachedValue.Value.IsLocked;
			}
			return false;
		}
	}

	public bool IsStrongbox
	{
		get
		{
			if (base.Address != 0L)
			{
				return _cachedValue.Value.IsStrongbox;
			}
			return false;
		}
	}

	private long StrongboxData => _cachedValue.Value.StrongboxData;

	public bool DestroyingAfterOpen
	{
		get
		{
			if (base.Address != 0L)
			{
				return _cachedValueStrongboxData.Value.DestroyingAfterOpen;
			}
			return false;
		}
	}

	public bool IsLarge
	{
		get
		{
			if (base.Address != 0L)
			{
				return _cachedValueStrongboxData.Value.IsLarge;
			}
			return false;
		}
	}

	public bool Stompable
	{
		get
		{
			if (base.Address != 0L)
			{
				return _cachedValueStrongboxData.Value.Stompable;
			}
			return false;
		}
	}

	public bool OpenOnDamage
	{
		get
		{
			if (base.Address != 0L)
			{
				return _cachedValueStrongboxData.Value.OpenOnDamage;
			}
			return false;
		}
	}

	public Chest()
	{
		_cachedValue = new FramesCache<ChestComponentOffsets>(() => base.M.Read<ChestComponentOffsets>(base.Address), 3u);
		_cachedValueStrongboxData = new FramesCache<StrongboxChestComponentData>(() => base.M.Read<StrongboxChestComponentData>(_cachedValue.Value.StrongboxData), 3u);
	}
}
