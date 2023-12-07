namespace ExileCore.PoEMemory.Elements.AtlasElements;

public class VoidStoneInventory : Element
{
	public VoidStoneSlot TopSlot => base[0].AsObject<VoidStoneSlot>();

	public VoidStoneSlot RightSlot => base[1].AsObject<VoidStoneSlot>();

	public VoidStoneSlot LeftSlot => base[2].AsObject<VoidStoneSlot>();

	public VoidStoneSlot BottomSlot => base[3].AsObject<VoidStoneSlot>();
}
