using System.Text;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects;

public class BestiaryRecipeComponent : RemoteMemoryObject
{
	private BestiaryCapturableMonster bestiaryCapturableMonster;

	private BestiaryFamily bestiaryFamily;

	private BestiaryGenus bestiaryGenus;

	private BestiaryGroup bestiaryGroup;

	private int minLevel = -1;

	private ModsDat.ModRecord mod;

	private string recipeId;

	public int Id { get; internal set; }

	public string RecipeId
	{
		get
		{
			if (recipeId == null)
			{
				return recipeId = base.M.ReadStringU(base.M.Read<long>(base.Address));
			}
			return recipeId;
		}
	}

	public int MinLevel
	{
		get
		{
			if (minLevel == -1)
			{
				return minLevel = base.M.Read<int>(base.Address + 8);
			}
			return minLevel;
		}
	}

	public BestiaryFamily BestiaryFamily
	{
		get
		{
			if (bestiaryFamily == null)
			{
				return bestiaryFamily = base.TheGame.Files.BestiaryFamilies.GetByAddress(base.M.Read<long>(base.Address + 20));
			}
			return bestiaryFamily;
		}
	}

	public BestiaryGroup BestiaryGroup
	{
		get
		{
			if (bestiaryGroup == null)
			{
				return bestiaryGroup = base.TheGame.Files.BestiaryGroups.GetByAddress(base.M.Read<long>(base.Address + 36));
			}
			return bestiaryGroup;
		}
	}

	public BestiaryGenus BestiaryGenus
	{
		get
		{
			if (bestiaryGenus == null)
			{
				return bestiaryGenus = base.TheGame.Files.BestiaryGenuses.GetByAddress(base.M.Read<long>(base.Address + 88));
			}
			return bestiaryGenus;
		}
	}

	public ModsDat.ModRecord Mod
	{
		get
		{
			if (mod == null)
			{
				return mod = base.TheGame.Files.Mods.GetModByAddress(base.M.Read<long>(base.Address + 52));
			}
			return mod;
		}
	}

	public BestiaryCapturableMonster BestiaryCapturableMonster => bestiaryCapturableMonster ?? (bestiaryCapturableMonster = base.TheGame.Files.BestiaryCapturableMonsters.GetByAddress(base.M.Read<long>(base.Address + 68)));

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
		handler.AppendLiteral("Id: ");
		handler.AppendFormatted(Id);
		handler.AppendLiteral(", ");
		stringBuilder3.Append(ref handler);
		if (MinLevel > 0)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder2);
			handler.AppendLiteral("MinLevel: ");
			handler.AppendFormatted(MinLevel);
			handler.AppendLiteral(", ");
			stringBuilder4.Append(ref handler);
		}
		if (Mod != null)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("Mod: ");
			handler.AppendFormatted(Mod);
			handler.AppendLiteral(", ");
			stringBuilder5.Append(ref handler);
		}
		if (BestiaryCapturableMonster != null)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder6 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(15, 1, stringBuilder2);
			handler.AppendLiteral("MonsterName: ");
			handler.AppendFormatted(BestiaryCapturableMonster.MonsterName);
			handler.AppendLiteral(", ");
			stringBuilder6.Append(ref handler);
		}
		if (BestiaryFamily != null)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(18, 1, stringBuilder2);
			handler.AppendLiteral("BestiaryFamily: ");
			handler.AppendFormatted(BestiaryFamily.Name);
			handler.AppendLiteral(", ");
			stringBuilder7.Append(ref handler);
		}
		if (BestiaryGroup != null)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder8 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
			handler.AppendLiteral("BestiaryGroup: ");
			handler.AppendFormatted(BestiaryGroup.Name);
			handler.AppendLiteral(", ");
			stringBuilder8.Append(ref handler);
		}
		if (BestiaryGenus != null)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder9 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
			handler.AppendLiteral("BestiaryGenus: ");
			handler.AppendFormatted(BestiaryGenus.Name);
			handler.AppendLiteral(", ");
			stringBuilder9.Append(ref handler);
		}
		return stringBuilder.ToString();
	}
}
