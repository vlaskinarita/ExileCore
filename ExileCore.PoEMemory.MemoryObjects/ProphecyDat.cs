namespace ExileCore.PoEMemory.MemoryObjects;

public class ProphecyDat : RemoteMemoryObject
{
	private string flavourText;

	private string id;

	private string name;

	private string predictionText;

	private string predictionText2;

	public int Index { get; set; }

	public string Id
	{
		get
		{
			if (id == null)
			{
				return id = base.M.ReadStringU(base.M.Read<long>(base.Address), 255);
			}
			return id;
		}
	}

	public string PredictionText
	{
		get
		{
			if (predictionText == null)
			{
				return predictionText = base.M.ReadStringU(base.M.Read<long>(base.Address + 8), 255);
			}
			return predictionText;
		}
	}

	public int ProphecyId => base.M.Read<int>(base.Address + 16);

	public string Name
	{
		get
		{
			if (name == null)
			{
				return name = base.M.ReadStringU(base.M.Read<long>(base.Address + 20));
			}
			return name;
		}
	}

	public string FlavourText
	{
		get
		{
			if (flavourText == null)
			{
				return flavourText = base.M.ReadStringU(base.M.Read<long>(base.Address + 28), 255);
			}
			return flavourText;
		}
	}

	public long ProphecyChainPtr => base.M.Read<long>(base.Address + 68);

	public int ProphecyChainPosition => base.M.Read<int>(base.Address + 76);

	public bool IsEnabled => base.M.Read<byte>(base.Address + 80) > 0;

	public int SealCost => base.M.Read<int>(base.Address + 81);

	public string PredictionText2
	{
		get
		{
			if (predictionText2 == null)
			{
				return predictionText2 = base.M.ReadStringU(base.M.Read<long>(base.Address + 85), 255);
			}
			return predictionText2;
		}
	}

	public override string ToString()
	{
		return Name + ", " + PredictionText;
	}
}
