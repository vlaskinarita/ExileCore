using System;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace ExileCore.PoEMemory.Elements;

public class HoverItemIcon : Element
{
	private const int HoveredItemTooltipOffset = 520;

	private const int ChatEntityArrayOffset = 1800;

	private ToolTipType? _tooltipType;

	[Obsolete("Use Element.Tooltip")]
	public Element InventoryItemTooltip => base.Tooltip;

	[Obsolete("Use Element.Tooltip")]
	public Element ItemInChatTooltip => base.Tooltip;

	public ItemOnGroundTooltip ToolTipOnGround => base.TheGame.IngameState.IngameUi.ItemOnGroundTooltip;

	[Obsolete]
	public int InventPosX => AsObject<NormalInventoryItem>().InventPosX;

	[Obsolete]
	public int InventPosY => AsObject<NormalInventoryItem>().InventPosY;

	public ToolTipType ToolTipType
	{
		get
		{
			try
			{
				ToolTipType valueOrDefault = _tooltipType.GetValueOrDefault();
				ToolTipType result;
				if (!_tooltipType.HasValue)
				{
					valueOrDefault = GetToolTipType();
					_tooltipType = valueOrDefault;
					result = valueOrDefault;
				}
				else
				{
					result = valueOrDefault;
				}
				return result;
			}
			catch (Exception ex)
			{
				Core.Logger?.Error(ex.Message + " " + ex.StackTrace);
				return ToolTipType.None;
			}
		}
	}

	public new Element Tooltip => ToolTipType switch
	{
		ToolTipType.ItemOnGround => ToolTipOnGround.Tooltip, 
		ToolTipType.InventoryItem => base.Tooltip, 
		ToolTipType.ItemInChat => base.Tooltip[0], 
		_ => null, 
	};

	public TooltipItemFrameElement ItemFrame => ((RemoteMemoryObject)(ToolTipType switch
	{
		ToolTipType.ItemOnGround => ToolTipOnGround.ItemFrame, 
		ToolTipType.InventoryItem => base.Tooltip[0], 
		ToolTipType.ItemInChat => base.Tooltip.GetChildFromIndices(0, 1), 
		_ => null, 
	}))?.AsObject<TooltipItemFrameElement>();

	public Element Item2DIcon => ToolTipType switch
	{
		ToolTipType.ItemOnGround => ToolTipOnGround.Item2DIcon, 
		ToolTipType.InventoryItem => null, 
		ToolTipType.ItemInChat => base.Tooltip.GetChildFromIndices(default(int), default(int)), 
		_ => null, 
	};

	public Entity Item => ToolTipType switch
	{
		ToolTipType.ItemOnGround => base.TheGame.IngameState.IngameUi.ItemsOnGroundLabelElement?.ItemOnHover?.GetComponent<WorldItem>()?.ItemEntity, 
		ToolTipType.InventoryItem => base.Entity, 
		ToolTipType.ItemInChat => base.Parent.ReadObjectAt<Entity>(1800), 
		_ => null, 
	};

	private ToolTipType GetToolTipType()
	{
		try
		{
			Element tooltip = base.Tooltip;
			if (tooltip != null && tooltip.IsVisible)
			{
				return (tooltip.ReadObjectAt<Element>(520).Address == tooltip[0]?.Address) ? ToolTipType.InventoryItem : ToolTipType.ItemInChat;
			}
			ItemOnGroundTooltip toolTipOnGround = ToolTipOnGround;
			if (toolTipOnGround != null && toolTipOnGround.Tooltip != null)
			{
				Element tooltipUI = toolTipOnGround.TooltipUI;
				if (tooltipUI != null && tooltipUI.IsVisible)
				{
					return ToolTipType.ItemOnGround;
				}
			}
		}
		catch (Exception value)
		{
			Core.Logger?.Error($"HoverItemIcon.cs -> {value}");
		}
		return ToolTipType.None;
	}
}
