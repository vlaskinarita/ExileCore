using System.IO;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.Components;

public class BlightTower : Component
{
	private string _iconFileName;

	private BlightTowerDat _info;

	public BlightTowerDat Info => _info ?? (_info = base.TheGame.Files.BlightTowers.GetByAddress(base.M.Read<long>(base.Address + 32)));

	public string Id => Info.Id;

	public string Name => Info.Name;

	public string Icon => Info.Icon;

	public string IconFileName => _iconFileName ?? (_iconFileName = Path.GetFileNameWithoutExtension(Icon));
}
