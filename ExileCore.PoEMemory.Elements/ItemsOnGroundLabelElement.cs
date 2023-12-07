using System.Collections.Generic;
using ExileCore.PoEMemory.MemoryObjects;
using GameOffsets;

namespace ExileCore.PoEMemory.Elements;

public class ItemsOnGroundLabelElement : Element
{
	private ItemsOnGroundLabelElementOffsets Data => base.M.Read<ItemsOnGroundLabelElementOffsets>(base.Address);

	public Element LabelOnHover
	{
		get
		{
			Element @object = GetObject<Element>(Data.LabelOnHoverPtr);
			if (@object.Address != 0L)
			{
				return @object;
			}
			return null;
		}
	}

	public Entity ItemOnHover
	{
		get
		{
			Entity @object = GetObject<Entity>(Data.ItemOnHoverPtr);
			if (@object.Address != 0L)
			{
				return @object;
			}
			return null;
		}
	}

	public string ItemOnHoverPath
	{
		get
		{
			if (ItemOnHover == null)
			{
				return "Null";
			}
			return ItemOnHover.Path;
		}
	}

	public string LabelOnHoverText
	{
		get
		{
			if (LabelOnHover == null)
			{
				return "Null";
			}
			return LabelOnHover.Text;
		}
	}

	public int CountLabels => base.M.Read<int>(base.Address + 688);

	public int CountLabels2 => base.M.Read<int>(base.Address + 752);

	public List<LabelOnGround> LabelsOnGround
	{
		get
		{
			long labelsOnGroundListPtr = Data.LabelsOnGroundListPtr;
			List<LabelOnGround> list = new List<LabelOnGround>();
			if (labelsOnGroundListPtr <= 0)
			{
				return null;
			}
			int num = 0;
			for (long num2 = base.M.Read<long>(labelsOnGroundListPtr); num2 != labelsOnGroundListPtr; num2 = base.M.Read<long>(num2))
			{
				LabelOnGround @object = GetObject<LabelOnGround>(num2);
				if (@object.Label.IsValid)
				{
					list.Add(@object);
				}
				num++;
				if (num > 5000)
				{
					return null;
				}
			}
			return list;
		}
	}
}
