namespace ExileCore.PoEMemory.Elements.ExpeditionElements;

public class TujenHaggleWindowElement : Element
{
	public string WindowTitle => GetChildAtIndex(0).Text;

	public Element HaggleTargetItem => GetChildAtIndex(1);

	public Element HaggleArtifactType => GetChildAtIndex(3);

	public int HaggleArtifactCurrentOfferAmount => int.Parse(GetChildAtIndex(4)?.Text ?? "0");

	public ArtifactSliderElement ArtifactOfferSliderElement => base[5].AsObject<ArtifactSliderElement>();

	public Element SameNewOfferIndicator => GetChildAtIndex(6);

	public Element ConfirmButton => GetChildAtIndex(7);

	public Element ExitWindowButton => GetChildAtIndex(8);

	public Element HaggleTargetItemTooltipElement => GetChildAtIndex(9);
}
