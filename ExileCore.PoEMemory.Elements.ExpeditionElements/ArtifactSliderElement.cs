namespace ExileCore.PoEMemory.Elements.ExpeditionElements;

public class ArtifactSliderElement : Element
{
	private const int CurrentOfferOffset = 644;

	private const int CurrentMinOfferOffset = 656;

	private const int CurrentMaxOfferOffset = 660;

	private const int MaxOfferOffset = 668;

	public int CurrentOffer
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 644);
		}
	}

	public int CurrentMinOffer
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 656);
		}
	}

	public int CurrentMaxOffer
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 660);
		}
	}

	public int MaxOffer
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 668);
		}
	}
}
