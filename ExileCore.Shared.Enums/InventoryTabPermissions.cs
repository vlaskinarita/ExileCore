using System;

namespace ExileCore.Shared.Enums;

[Flags]
public enum InventoryTabPermissions : uint
{
	Add = 2u,
	None = 0u,
	Remove = 4u,
	View = 1u
}
