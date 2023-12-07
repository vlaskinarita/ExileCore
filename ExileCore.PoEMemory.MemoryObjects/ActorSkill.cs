using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ActorSkill : RemoteMemoryObject
{
	private readonly CachedValue<ActorSkillCooldown> _actorSkillCooldown;

	private readonly CachedValue<ActorSkillOffsets> _cache;

	private readonly CachedValue<Dictionary<GameStat, int>> _statsCache;

	private readonly CachedValue<ActorVaalSkill> _actorVaalSkill;

	private ActorSkillOffsets Struct => _cache.Value;

	public ushort Id => Struct.SubData.Id;

	public GrantedEffectsPerLevel EffectsPerLevel => GetObject<GrantedEffectsPerLevel>(Struct.SubData.EffectsPerLevelPtr);

	public bool CanBeUsedWithWeapon => Struct.SubData.CanBeUsedWithWeapon > 0;

	public bool CanBeUsed
	{
		get
		{
			if (Struct.SubData.CannotBeUsed == 0 && !IsOnCooldown)
			{
				return HasEnoughSouls;
			}
			return false;
		}
	}

	public int Cost => GetStat(GameStat.ManaCost);

	public bool IsChanneling => Struct.CastType == 10;

	public int TotalUses => Struct.SubData.TotalUses;

	public float Cooldown => (float)Struct.SubData.Cooldown / 1000f / ((float)(100 + GetStat(GameStat.VirtualCooldownSpeedPct)) * 0.01f);

	public int SoulsPerUse => Struct.SubData.SoulsPerUse;

	public int TotalVaalUses => Struct.SubData.TotalVaalUses;

	public bool IsOnSkillBar => SkillSlotIndex != -1;

	public int SkillSlotIndex => base.TheGame.IngameState.Data.ServerData.SkillBarIds.IndexOf(Id);

	public byte SkillUseStage => Struct.SkillUseStage;

	internal int SlotIdentifier => (Id >> 8) & 0xFF;

	public int SocketIndex => (SlotIdentifier >> 2) & 0xF;

	public bool IsUserSkill => (SlotIdentifier & 0x80) > 0;

	public bool AllowedToCast
	{
		get
		{
			if (CanBeUsedWithWeapon)
			{
				return CanBeUsed;
			}
			return false;
		}
	}

	public bool IsUsingOrCharging => SkillUseStage >= 2;

	public bool IsUsing => SkillUseStage > 1;

	public bool PrepareForUsage => SkillUseStage == 1;

	public float Dps => (float)GetStat((GameStat)(677 + (IsUsing ? 4 : 0))) / 100f;

	public bool IsSpell => GetStat(GameStat.CastingSpell) == 1;

	public bool IsAttack => GetStat(GameStat.SkillIsAttack) == 1;

	public bool IsCry => InternalName.EndsWith("_cry");

	public bool IsBallistaTotem => InternalName.EndsWith("_ballista_totem");

	public bool IsInstant => GetStat(GameStat.SkillIsInstant) == 1;

	public bool IsMine
	{
		get
		{
			if (GetStat(GameStat.IsRemoteMine) != 1)
			{
				return GetStat(GameStat.SkillIsMined) == 1;
			}
			return true;
		}
	}

	public bool IsTotem
	{
		get
		{
			if (GetStat(GameStat.IsTotem) != 1)
			{
				return GetStat(GameStat.SkillIsTotemified) == 1;
			}
			return true;
		}
	}

	public bool IsTrap
	{
		get
		{
			if (GetStat(GameStat.IsTrap) != 1)
			{
				return GetStat(GameStat.SkillIsTrapped) == 1;
			}
			return true;
		}
	}

	public bool IsVaalSkill => SoulsPerUse > 0;

	public Dictionary<GameStat, int> Stats => _statsCache.Value;

	public Actor Actor { get; private set; }

	public string Name
	{
		get
		{
			ushort id = Id;
			GrantedEffectsPerLevel effectsPerLevel = EffectsPerLevel;
			if (effectsPerLevel != null && effectsPerLevel.Address != 0L)
			{
				SkillGemWrapper skillGemWrapper = effectsPerLevel.SkillGemWrapper;
				if (!string.IsNullOrEmpty(skillGemWrapper.Name))
				{
					return skillGemWrapper.Name;
				}
				if (!string.IsNullOrEmpty(skillGemWrapper.ActiveSkill.InternalName))
				{
					return Id.ToString(CultureInfo.InvariantCulture);
				}
				return skillGemWrapper.ActiveSkill.InternalName;
			}
			return id switch
			{
				614 => "Interaction", 
				10505 => "Move", 
				14297 => "WashedUp", 
				_ => InternalName, 
			};
		}
	}

	public TimeSpan CastTime
	{
		get
		{
			if (IsBallistaTotem)
			{
				return TimeSpan.FromMilliseconds(350.0);
			}
			if (IsTotem)
			{
				return TimeSpan.FromMilliseconds(600.0);
			}
			int hundredTimesAttacksPerSecond = HundredTimesAttacksPerSecond;
			return TimeSpan.FromMilliseconds((!IsInstant && hundredTimesAttacksPerSecond != 0) ? ((int)Math.Ceiling(1000f / ((float)hundredTimesAttacksPerSecond * 0.01f))) : 0);
		}
	}

	public int HundredTimesAttacksPerSecond
	{
		get
		{
			if (IsInstant)
			{
				return 0;
			}
			int num = (IsSpell ? GetStat(GameStat.HundredTimesCastsPerSecond) : ((!IsAttack) ? GetStat(GameStat.HundredTimesNonSpellCastsPerSecond) : GetStat(GameStat.HundredTimesAttacksPerSecond)));
			if (num == 0 && IsCry)
			{
				num = 60;
			}
			return num;
		}
	}

	public bool IsOnCooldown
	{
		get
		{
			ActorSkillCooldown value = _actorSkillCooldown.Value;
			if (value == null)
			{
				return false;
			}
			return value.SkillCooldowns.Count >= value.MaxUses;
		}
	}

	public bool HasEnoughSouls
	{
		get
		{
			if (!IsVaalSkill)
			{
				return true;
			}
			ActorVaalSkill value = _actorVaalSkill.Value;
			if (value == null)
			{
				return true;
			}
			return value.CurrVaalSouls >= value.VaalMaxSouls;
		}
	}

	public int RemainingUses
	{
		get
		{
			ActorSkillCooldown value = _actorSkillCooldown.Value;
			if (value == null)
			{
				return 0;
			}
			return value.MaxUses - value.SkillCooldowns.Count;
		}
	}

	public string InternalName
	{
		get
		{
			GrantedEffectsPerLevel effectsPerLevel = EffectsPerLevel;
			if (effectsPerLevel != null)
			{
				return effectsPerLevel.SkillGemWrapper.ActiveSkill.InternalName;
			}
			return Id switch
			{
				614 => "Interaction", 
				10505 => "Move", 
				14297 => "WashedUp", 
				_ => Id.ToString(CultureInfo.InvariantCulture), 
			};
		}
	}

	public List<DeployedObject> DeployedObjects => Actor.DeployedObjects.FindAll((DeployedObject x) => x.SkillKey == Id);

	public ActorSkill()
	{
		_actorSkillCooldown = new AreaCache<ActorSkillCooldown>(() => Actor.ActorSkillsCooldowns.FirstOrDefault((ActorSkillCooldown x) => x.Id == Id && x.SkillSubId == EffectsPerLevel.SkillGemWrapper.ActiveSkillSubId));
		_actorVaalSkill = new AreaCache<ActorVaalSkill>(() => Actor.ActorVaalSkills.FirstOrDefault((ActorVaalSkill x) => x.VaalSkillInternalName == InternalName));
		_cache = new FrameCache<ActorSkillOffsets>(() => base.M.Read<ActorSkillOffsets>(base.Address));
		_statsCache = new FrameCache<Dictionary<GameStat, int>>(() => ReadStats(Struct.SubData.StatsPtr));
	}

	public ActorSkill SetActor(Actor actor)
	{
		Actor = actor;
		return this;
	}

	private Dictionary<GameStat, int> ReadStats(long address)
	{
		SubStatsComponentOffsets subStatsComponentOffsets = base.M.Read<SubStatsComponentOffsets>(address);
		return base.M.ReadStdVector<(GameStat, int)>(subStatsComponentOffsets.Stats).ToDictionary(((GameStat Stat, int Value) x) => x.Stat, ((GameStat Stat, int Value) x) => x.Value);
	}

	public int GetStat(GameStat stat)
	{
		if (Stats.TryGetValue(stat, out var value))
		{
			return value;
		}
		return 0;
	}

	public override string ToString()
	{
		return $"IsUsing: {IsUsing}, {Name}, Id: {Id}, InternalName: {InternalName}, CanBeUsed: {CanBeUsed}";
	}
}
