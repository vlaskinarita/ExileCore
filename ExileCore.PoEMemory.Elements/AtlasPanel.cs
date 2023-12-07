using System.Collections.Generic;
using ExileCore.PoEMemory.Elements.AtlasElements;

namespace ExileCore.PoEMemory.Elements;

public class AtlasPanel : Element
{
	public Element AtlasInventory => GetObject<Element>(base.M.Read<long>(base.Address + 584, new int[1] { 944 }));

	public IList<Element> InventorySlots => AtlasInventory.Children;

	public AtlasMasterMissionPanelElement MasterMissionPanel => base[3].AsObject<AtlasMasterMissionPanelElement>();

	public VoidStoneFavouriteMapPanelElement VoidStoneAndFavouriteMapPanel => base[4].AsObject<VoidStoneFavouriteMapPanelElement>();

	public Element SearchbarPanel => GetChildAtIndex(6);

	public Element AtlasSkillsToggleButton => GetChildAtIndex(7);

	public Element KiracsVaultPassButton => GetChildAtIndex(8);

	public Element InnerAtlas => GetChildAtIndex(0);

	public Dictionary<Atlasbonus, int> AtlasBonus => new Dictionary<Atlasbonus, int>
	{
		{
			Atlasbonus.Minimum,
			0
		},
		{
			Atlasbonus.Current,
			int.Parse(InnerAtlas.GetChildAtIndex(120).Text.Split('/')[0])
		},
		{
			Atlasbonus.Maximum,
			int.Parse(InnerAtlas.GetChildAtIndex(120).Text.Split('/')[1])
		}
	};

	public Element SearingExarchCounterElement => InnerAtlas.GetChildAtIndex(121);

	public Element MavenCounterElement => InnerAtlas.GetChildAtIndex(122);

	public Element EaterofWorldsCounterElement => InnerAtlas.GetChildAtIndex(123);

	public VoidStoneInventory SocketedVoidstones => InnerAtlas.GetChildFromIndices(124, 0).AsObject<VoidStoneInventory>();
}
