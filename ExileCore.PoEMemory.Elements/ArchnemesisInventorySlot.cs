using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.Elements;

public class ArchnemesisInventorySlot : Element
{
	private const int ItemOffset = 992;

	public bool HasItem => base.M.Read<long>(base.Address + 992) != 0;

	public ArchnemesisMod Item => base.TheGame.Files.ArchnemesisMods.GetByAddress(base.M.Read<long>(base.Address + 992));
}
