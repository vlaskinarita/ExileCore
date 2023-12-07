using System;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.Models;

public sealed class ItemStats
{
	private static StatTranslator translate;

	private readonly Entity item;

	private readonly float[] stats;

	public ItemStats(Entity item)
	{
		this.item = item;
		if (translate == null)
		{
			translate = new StatTranslator();
		}
		stats = new float[Enum.GetValues<ItemStatEnum>().Length];
		ParseSockets();
		ParseExplicitMods();
		if (item.HasComponent<Weapon>())
		{
			ParseWeaponStats();
		}
	}

	private void ParseWeaponStats()
	{
		Weapon component = item.GetComponent<Weapon>();
		float num = (float)(component.DamageMin + component.DamageMax) / 2f + GetStat(ItemStatEnum.LocalPhysicalDamage);
		num *= 1f + (GetStat(ItemStatEnum.LocalPhysicalDamagePercent) + (float)item.GetComponent<Quality>().ItemQuality) / 100f;
		AddToMod(ItemStatEnum.AveragePhysicalDamage, num);
		float num2 = 1f / ((float)component.AttackTime / 1000f);
		num2 *= 1f + GetStat(ItemStatEnum.LocalAttackSpeed) / 100f;
		AddToMod(ItemStatEnum.AttackPerSecond, num2);
		float num3 = (float)component.CritChance / 100f;
		num3 *= 1f + GetStat(ItemStatEnum.LocalCritChance) / 100f;
		AddToMod(ItemStatEnum.WeaponCritChance, num3);
		float num4 = GetStat(ItemStatEnum.LocalAddedColdDamage) + GetStat(ItemStatEnum.LocalAddedFireDamage) + GetStat(ItemStatEnum.LocalAddedLightningDamage);
		AddToMod(ItemStatEnum.AverageElementalDamage, num4);
		AddToMod(ItemStatEnum.DPS, (num + num4) * num2);
		AddToMod(ItemStatEnum.PhysicalDPS, num * num2);
	}

	private void ParseExplicitMods()
	{
		foreach (ItemMod itemMod in item.GetComponent<Mods>().ItemMods)
		{
			translate.Translate(this, itemMod);
		}
		AddToMod(ItemStatEnum.ElementalResistance, GetStat(ItemStatEnum.LightningResistance) + GetStat(ItemStatEnum.FireResistance) + GetStat(ItemStatEnum.ColdResistance));
		AddToMod(ItemStatEnum.TotalResistance, GetStat(ItemStatEnum.ElementalResistance) + GetStat(ItemStatEnum.TotalResistance));
	}

	private void ParseSockets()
	{
	}

	public void AddToMod(ItemStatEnum stat, float value)
	{
		stats[(int)stat] += value;
	}

	public float GetStat(ItemStatEnum stat)
	{
		return stats[(int)stat];
	}
}
