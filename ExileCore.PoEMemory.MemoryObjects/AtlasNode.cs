using ExileCore.PoEMemory.FilesInMemory.Atlas;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects;

public class AtlasNode : RemoteMemoryObject
{
	private WorldArea area;

	private float posX = -1f;

	private float posY = -1f;

	private string text;

	private const int TIER_LAYERS = 161;

	public WorldArea Area => area ?? (area = base.TheGame.Files.WorldAreas.GetByAddress(base.M.Read<long>(base.Address)));

	public Vector2 PosL0 => GetPosByLayer(0);

	public Vector2 PosL1 => GetPosByLayer(1);

	public Vector2 PosL2 => GetPosByLayer(2);

	public Vector2 PosL3 => GetPosByLayer(3);

	public Vector2 PosL4 => GetPosByLayer(4);

	public Vector2 DefaultPos => PosL0;

	public float PosX => posX;

	public float PosY => posY;

	public byte Flag0 => base.M.Read<byte>(base.Address + 32);

	public string FlavourText => text ?? (text = base.M.ReadStringU(base.M.Read<long>(base.M.Read<long>(base.Address + 49) + 12)));

	public AtlasRegion AtlasRegion => base.TheGame.Files.AtlasRegions.GetByAddress(base.M.Read<long>(base.Address + 65));

	public bool IsUniqueMap
	{
		get
		{
			string text = base.M.ReadStringU(base.M.Read<long>(base.Address + 16, new int[1]));
			if (!string.IsNullOrEmpty(text))
			{
				return text.StartsWith("Uniq");
			}
			return false;
		}
	}

	public Vector2 GetPosByLayer(int layer)
	{
		float x = base.M.Read<float>(base.Address + 181 + layer * 4);
		float y = base.M.Read<float>(base.Address + 201 + layer * 4);
		return new Vector2(x, y);
	}

	public int GetTierByLayer(int layer)
	{
		return base.M.Read<int>(base.Address + 161 + layer * 4);
	}

	public int GetLayerByTier(int tier)
	{
		for (int i = 0; i < 5; i++)
		{
			if (base.M.Read<int>(base.Address + 161 + i * 4) == tier)
			{
				return i;
			}
		}
		return -1;
	}

	public override string ToString()
	{
		return $"{Area?.Name}, PosX: {PosX}, PosY: {PosY}, Text: {FlavourText}";
	}
}
