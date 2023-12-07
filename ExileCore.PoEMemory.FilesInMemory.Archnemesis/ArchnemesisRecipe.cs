using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.FilesInMemory.Archnemesis;

public class ArchnemesisRecipe : RemoteMemoryObject
{
	private ArchnemesisMod _outcome;

	private List<ArchnemesisMod> _components;

	public ArchnemesisMod Outcome => _outcome ?? (_outcome = base.TheGame.Files.ArchnemesisMods.GetByAddress(base.M.Read<long>(base.Address)));

	public List<ArchnemesisMod> Components
	{
		get
		{
			if (_components == null)
			{
				_components = new List<ArchnemesisMod>();
				long num = base.M.Read<long>(base.Address + 24);
				for (int i = 0; i < base.M.Read<int>(base.Address + 16); i++)
				{
					long addr = num + i * 2 * 8;
					long address = base.M.Read<long>(addr);
					Components.Add(base.TheGame.Files.ArchnemesisMods.GetByAddress(address));
				}
			}
			return _components;
		}
	}

	public override string ToString()
	{
		return Outcome.DisplayName + " (" + string.Join(", ", Components.Select((ArchnemesisMod x) => x.DisplayName)) + ")";
	}
}
