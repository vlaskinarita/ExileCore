using System;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class Life : Component
{
	private readonly CachedValue<LifeComponentOffsets> _life;

	public new long OwnerAddress => LifeComponentOffsetsStruct.Owner;

	private LifeComponentOffsets LifeComponentOffsetsStruct => _life.Value;

	public VitalStruct Health => _life.Value.Health;

	public VitalStruct Mana => _life.Value.Mana;

	public VitalStruct EnergyShield => _life.Value.EnergyShield;

	public int MaxHP
	{
		get
		{
			if (base.Address == 0L)
			{
				return 1;
			}
			return LifeComponentOffsetsStruct.Health.Max;
		}
	}

	public int CurHP
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return LifeComponentOffsetsStruct.Health.Current;
		}
	}

	public int ReservedFlatHP => LifeComponentOffsetsStruct.Health.ReservedFlat;

	public int ReservedPercentHP => LifeComponentOffsetsStruct.Health.ReservedFraction / 100;

	public int MaxMana
	{
		get
		{
			if (base.Address == 0L)
			{
				return 1;
			}
			return LifeComponentOffsetsStruct.Mana.Max;
		}
	}

	public int CurMana
	{
		get
		{
			if (base.Address == 0L)
			{
				return 1;
			}
			return LifeComponentOffsetsStruct.Mana.Current;
		}
	}

	public int ReservedFlatMana => LifeComponentOffsetsStruct.Mana.ReservedFlat;

	public int ReservedPercentMana => LifeComponentOffsetsStruct.Mana.ReservedFraction / 100;

	public int MaxES => LifeComponentOffsetsStruct.EnergyShield.Max;

	public int CurES => LifeComponentOffsetsStruct.EnergyShield.Current;

	public float HPPercentage => (float)CurHP / (float)((double)(MaxHP - ReservedFlatHP) - Math.Round((double)ReservedPercentHP * 0.01 * (double)MaxHP));

	public float MPPercentage => (float)CurMana / (float)((double)(MaxMana - ReservedFlatMana) - Math.Round((double)ReservedPercentMana * 0.01 * (double)MaxMana));

	public float ESPercentage
	{
		get
		{
			if (MaxES != 0)
			{
				return (float)CurES / (float)MaxES;
			}
			return 0f;
		}
	}

	public Life()
	{
		_life = new FrameCache<LifeComponentOffsets>(() => (base.Address != 0L) ? base.M.Read<LifeComponentOffsets>(base.Address) : default(LifeComponentOffsets));
	}
}
