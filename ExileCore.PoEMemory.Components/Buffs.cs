using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Cache;
using GameOffsets;
using GameOffsets.Native;
using JM.LinqFaster;

namespace ExileCore.PoEMemory.Components;

public sealed class Buffs : Component
{
	private readonly CachedValue<List<Buff>> _cachedValueBuffs;

	public List<Buff> BuffsList => _cachedValueBuffs.Value;

	public Buffs()
	{
		_cachedValueBuffs = new FrameCache<List<Buff>>(CacheUtils.RememberLastValue<List<Buff>>(ParseBuffs));
	}

	public List<Buff> ParseBuffs()
	{
		return ParseBuffs(null);
	}

	private List<Buff> ParseBuffs(List<Buff> lastValue)
	{
		NativePtrArray buffs = base.M.Read<BuffsOffsets>(base.Address).Buffs;
		List<Buff> list = base.M.ReadPointersArray(buffs.First, buffs.Last).Select(base.GetObject<Buff>).ToList();
		if (list.Count != 0 || lastValue == null || lastValue.Count <= 0 || !buffs.Equals(default(NativePtrArray)))
		{
			return list;
		}
		return lastValue;
	}

	public bool HasBuff(string buff)
	{
		return BuffsList?.AnyF((Buff x) => x.Name == buff) ?? false;
	}

	public bool TryGetBuff(string name, out Buff buff)
	{
		buff = BuffsList.FirstOrDefault((Buff x) => x.Name == name);
		return buff != null;
	}
}
