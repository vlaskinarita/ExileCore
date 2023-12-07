using System.Collections.Generic;
using ExileCore.Shared.Cache;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ActorSkillCooldown : RemoteMemoryObject
{
	private readonly CachedValue<ActorSkillCooldownOffsets> _cache;

	public ushort Id => _cache.Value.SkillId;

	public int SkillSubId => _cache.Value.SkillSubId;

	private StdVector CooldownUses => _cache.Value.Cooldowns;

	public int MaxUses => _cache.Value.MaxUses;

	public List<SkillCooldown> SkillCooldowns => base.M.ReadStructsArray<SkillCooldown>(CooldownUses.First, CooldownUses.Last, 16, null);

	public ActorSkillCooldown()
	{
		_cache = new FrameCache<ActorSkillCooldownOffsets>(() => base.M.Read<ActorSkillCooldownOffsets>(base.Address));
	}
}
