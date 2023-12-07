using ExileCore.PoEMemory.Models;

namespace ExileCore.PoEMemory.FilesInMemory.Sanctum;

public class SanctumDeferredRewardCategory : RemoteMemoryObject
{
	public BaseItemType BaseType => base.TheGame.Files.BaseItemTypes.GetFromAddress(base.M.Read<long>(base.Address));

	public string CurrencyName => base.M.ReadStringU(base.M.Read<long>(base.Address + 16));

	public override string ToString()
	{
		return CurrencyName;
	}
}
