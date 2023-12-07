using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory.Ancestor;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.MemoryObjects;

public class AncestorFightOption : RemoteMemoryObject
{
	private AncestralTrialTribe _tribeToFight;

	private List<AncestorFightOptionReward> _rewards;

	public AncestralTrialTribe TribeToFight => _tribeToFight ?? (_tribeToFight = base.TheGame.Files.AncestralTrialTribes.GetByAddress(base.M.Read<long>(base.Address)));

	public List<AncestorFightOptionReward> Rewards
	{
		get
		{
			if (_rewards != null)
			{
				return _rewards;
			}
			List<AncestorFightOptionReward> list = (from x in base.M.ReadStdVectorStride<(long, long, int)>(base.M.Read<StdVector>(base.Address + 24), 24)
				select new AncestorFightOptionReward(base.TheGame.Files.AncestralTrialTribes.GetByAddress(x.TribePtr), x.Amount)).ToList();
			if (list.All((AncestorFightOptionReward x) => x.FavorTribe != null))
			{
				_rewards = list;
			}
			return list;
		}
	}
}
