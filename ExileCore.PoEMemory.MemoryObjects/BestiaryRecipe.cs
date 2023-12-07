using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class BestiaryRecipe : RemoteMemoryObject
{
	private List<BestiaryRecipeComponent> _components;

	private string _description;

	private string _notes;

	private string _recipeId;

	private BestiaryRecipeComponent _specialMonster;

	public int Id { get; internal set; }

	public string RecipeId => _recipeId ?? (_recipeId = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string Description => _description ?? (_description = base.M.ReadStringU(base.M.Read<long>(base.Address + 8)));

	public string Notes => _notes ?? (_notes = base.M.ReadStringU(base.M.Read<long>(base.Address + 32)));

	[Obsolete]
	public string HintText => "";

	public bool RequireSpecialMonster => Components.Count == 4;

	public BestiaryRecipeComponent SpecialMonster
	{
		get
		{
			if (!RequireSpecialMonster)
			{
				return null;
			}
			return _specialMonster ?? (_specialMonster = Components.FirstOrDefault());
		}
	}

	public IList<BestiaryRecipeComponent> Components => _components ?? (_components = base.M.Read<DatArrayStruct>(base.Address + 16).ReadDatPtr(base.M).Select(base.TheGame.Files.BestiaryRecipeComponents.GetByAddress)
		.ToList());

	public override string ToString()
	{
		return RecipeId + ": " + Description;
	}
}
