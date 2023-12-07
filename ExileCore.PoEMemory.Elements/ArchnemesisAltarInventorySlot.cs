using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.Elements;

public class ArchnemesisAltarInventorySlot : Element
{
	private const int ItemOffset = 576;

	public bool HasItem => base.M.Read<long>(base.Address + 576) != 0;

	public ArchnemesisMod Item => base.TheGame.Files.ArchnemesisMods.GetByAddress(base.M.Read<long>(base.Address + 576));
}
