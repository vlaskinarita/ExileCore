using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory.Archnemesis;

namespace ExileCore.PoEMemory.FilesInMemory;

public class ArchnemesisMod : RemoteMemoryObject
{
	private string _displayName;

	private string _internalName;

	private string _description;

	private string _iconPath;

	private string _formatString;

	private List<string> _rewards;

	private ArchnemesisRecipe _recipe;

	private bool _recipeChecked;

	public string DisplayName => _displayName ?? (_displayName = base.M.ReadStringU(base.M.Read<long>(base.Address + 106, new int[1] { 16 })));

	public string Description => _description ?? (_description = base.M.ReadStringU(base.M.Read<long>(base.Address + 98)));

	public string InternalName => _internalName ?? (_internalName = base.M.ReadStringU(base.M.Read<long>(base.Address + 66, new int[1])));

	public string IconPath => _iconPath ?? (_iconPath = base.M.ReadStringU(base.M.Read<long>(base.Address + 56)));

	public string FormatString => _formatString ?? (_formatString = base.M.ReadStringU(base.M.Read<long>(base.Address + 48)));

	public List<string> Rewards
	{
		get
		{
			if (_rewards == null)
			{
				_rewards = new List<string>();
				long num = base.M.Read<long>(base.Address + 24);
				for (int i = 0; i < base.M.Read<int>(base.Address + 16); i++)
				{
					long addr = num + i * 2 * 8;
					Rewards.Add(base.M.ReadStringU(base.M.Read<long>(base.M.Read<long>(addr))));
				}
			}
			return _rewards;
		}
	}

	public ArchnemesisRecipe Recipe
	{
		get
		{
			if (!_recipeChecked)
			{
				_recipeChecked = true;
				_recipe = base.TheGame.Files.ArchnemesisRecipes.EntriesList.FirstOrDefault((ArchnemesisRecipe x) => x.Outcome.Address == base.Address);
			}
			return _recipe;
		}
	}

	public override string ToString()
	{
		return DisplayName + " (" + string.Join(", ", Rewards) + ")";
	}
}
