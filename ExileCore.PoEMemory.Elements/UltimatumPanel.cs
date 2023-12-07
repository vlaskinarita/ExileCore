using System.Collections.Generic;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.FilesInMemory.Ultimatum;

namespace ExileCore.PoEMemory.Elements;

public class UltimatumPanel : Element
{
	public Element ChoisesPanel => GetChildFromIndices(2, 4);

	public int SlectedChoise => base.M.Read<int>(ChoisesPanel.Address + 584);

	public UltimatumModifier[] Modifiers
	{
		get
		{
			long num = base.M.Read<long>(ChoisesPanel.Address + 560);
			return new UltimatumModifier[3]
			{
				ReadObject<UltimatumModifier>(num),
				ReadObject<UltimatumModifier>(num + 16),
				ReadObject<UltimatumModifier>(num + 32)
			};
		}
	}

	public IList<Element> ChoisesElements => ChoisesPanel.GetChildAtIndex(0).Children;

	public Element InventoryElement => GetObject<Element>(base.M.Read<long>(base.Address + 464, new int[1] { 624 }));

	public NormalInventoryItem NextRewardItem => InventoryElement.GetChildAtIndex(1).AsObject<NormalInventoryItem>();
}
