using System;

namespace ExileCore.Shared.Enums;

[Flags]
public enum ActionFlags
{
	None = 0,
	UsingAbility = 2,
	AbilityCooldownActive = 0x10,
	UsingAbilityAbilityCooldown = 0x12,
	Dead = 0x40,
	Moving = 0x80,
	WashedUpState = 0x100,
	HasMines = 0x800
}
