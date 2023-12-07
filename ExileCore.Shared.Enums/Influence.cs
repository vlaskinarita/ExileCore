using System;

namespace ExileCore.Shared.Enums;

[Flags]
public enum Influence : byte
{
	None = 0,
	Shaper = 1,
	Elder = 2,
	Crusader = 4,
	Redeemer = 8,
	Hunter = 0x10,
	Warlord = 0x20
}
