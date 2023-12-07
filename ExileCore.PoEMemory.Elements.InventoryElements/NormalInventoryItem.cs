using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.Elements.InventoryElements;

public class NormalInventoryItem : Element
{
	private Entity _item;

	private readonly Lazy<NormalInventoryItemOffsets> cachedValue;

	[Obsolete]
	public virtual int InventPosX => 0;

	[Obsolete]
	public virtual int InventPosY => 0;

	public virtual int ItemWidth => cachedValue.Value.Width;

	public virtual int ItemHeight => cachedValue.Value.Height;

	public Entity Item => _item ?? (_item = GetObject<Entity>(cachedValue.Value.Item));

	public ToolTipType toolTipType => ToolTipType.InventoryItem;

	public NormalInventoryItem()
	{
		cachedValue = new Lazy<NormalInventoryItemOffsets>(() => base.M.Read<NormalInventoryItemOffsets>(base.Address));
	}
}
