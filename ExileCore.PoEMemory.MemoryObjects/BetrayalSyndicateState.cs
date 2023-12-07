using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects;

public class BetrayalSyndicateState : RemoteMemoryObject
{
	public static int STRUCT_SIZE = 160;

	public Element UIElement => ReadObjectAt<Element>(0);

	public float PosX => base.M.Read<float>(base.Address + 124);

	public float PosY => base.M.Read<float>(base.Address + 124);

	public BetrayalTarget Target => base.TheGame.Files.BetrayalTargets.GetByAddress(base.M.Read<long>(base.Address + 8));

	public BetrayalJob Job => base.TheGame.Files.BetrayalJobs.GetByAddress(base.M.Read<long>(base.Address + 24));

	public BetrayalRank Rank => base.TheGame.Files.BetrayalRanks.GetByAddress(base.M.Read<long>(base.Address + 40));

	public BetrayalReward Reward => base.TheGame.Files.BetrayalRewards.EntriesList.Find((BetrayalReward x) => x.Target == Target && x.Job == Job && x.Rank == Rank);

	public List<BetrayalUpgrade> BetrayalUpgrades
	{
		get
		{
			long num = base.M.Read<long>(base.Address + 56);
			long num2 = base.M.Read<long>(base.Address + 64);
			List<BetrayalUpgrade> list = new List<BetrayalUpgrade>();
			for (long num3 = num; num3 < num2; num3 += 16)
			{
				list.Add(ReadObject<BetrayalUpgrade>(num3 + 8));
			}
			return list;
		}
	}

	public List<BetrayalSyndicateState> Relations
	{
		get
		{
			long num = base.M.Read<long>(base.Address + 80);
			List<BetrayalSyndicateState> list = new List<BetrayalSyndicateState>();
			for (int i = 0; i < 3; i++)
			{
				long num2 = base.M.Read<long>(num + i * 8);
				if (num2 != 0L)
				{
					list.Add(GetObject<BetrayalSyndicateState>(num2));
				}
			}
			return list;
		}
	}

	public override string ToString()
	{
		return $"{Target?.Name}, {Rank?.Name}, {Job?.Name}";
	}
}
