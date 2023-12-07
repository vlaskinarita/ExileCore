using System;

namespace ExileCore.Shared.Enums;

[Flags]
public enum InventoryTabFlags : byte
{
	Hidden = 0x80,
	MapSeries = 0x40,
	Premium = 4,
	Public = 0x20,
	RemoveOnly = 1,
	Unknown1 = 0x10,
	Unknown2 = 2,
	Unknown3 = 8
}
