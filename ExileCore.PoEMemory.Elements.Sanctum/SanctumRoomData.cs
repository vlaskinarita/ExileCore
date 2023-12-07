using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory.Sanctum;

namespace ExileCore.PoEMemory.Elements.Sanctum;

public class SanctumRoomData : RemoteMemoryObject
{
	public SanctumRoom FightRoom => base.TheGame.Files.SanctumRooms.GetByAddress(base.M.Read<long>(base.Address + 8));

	public SanctumRoom RewardRoom => base.TheGame.Files.SanctumRooms.GetByAddress(base.M.Read<long>(base.Address + 24));

	public SanctumPersistentEffect RoomEffect => base.TheGame.Files.SanctumPersistentEffects.GetByAddress(base.M.Read<long>(base.Address + 40));

	public SanctumDeferredRewardCategory Reward1 => base.TheGame.Files.SanctumDeferredRewardCategories.GetByAddress(base.M.Read<long>(base.Address + 64));

	public SanctumDeferredRewardCategory Reward2 => base.TheGame.Files.SanctumDeferredRewardCategories.GetByAddress(base.M.Read<long>(base.Address + 80));

	public SanctumDeferredRewardCategory Reward3 => base.TheGame.Files.SanctumDeferredRewardCategories.GetByAddress(base.M.Read<long>(base.Address + 96));

	public List<SanctumDeferredRewardCategory> Rewards => new SanctumDeferredRewardCategory[3] { Reward1, Reward2, Reward3 }.Where((SanctumDeferredRewardCategory x) => x != null && x.Address != 0).ToList();
}
