using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements.AtlasElements;

public class VoidStoneSlot : Element
{
	public bool isEmpty => GetChildAtIndex(1) == null;

	public NormalInventoryItem Voidstone => base[1].AsObject<NormalInventoryItem>();

	public bool hasSextantApplied => Voidstone.Item.HasComponent<Mods>();

	public ItemMod SextantMod
	{
		get
		{
			if (!hasSextantApplied)
			{
				return null;
			}
			return Voidstone.Item.GetComponent<Mods>().ItemMods[0];
		}
	}

	public int RemainingSextantCharges => SextantMod?.Values[0] ?? 0;
}
