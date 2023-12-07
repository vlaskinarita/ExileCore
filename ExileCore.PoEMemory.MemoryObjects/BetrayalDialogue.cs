using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;

namespace ExileCore.PoEMemory.MemoryObjects;

public class BetrayalDialogue : RemoteMemoryObject
{
	public BetrayalTarget Target => base.TheGame.Files.BetrayalTargets.GetByAddress(base.M.Read<long>(base.Address + 8));

	public int Unknown1 => base.M.Read<int>(base.Address + 16);

	public int Unknown2 => base.M.Read<int>(base.Address + 20);

	public int Unknown3 => base.M.Read<int>(base.Address + 56);

	public bool Unknown4 => base.M.Read<byte>(base.Address + 108) > 0;

	public bool Unknown5 => base.M.Read<byte>(base.Address + 141) > 0;

	public BetrayalJob Job => base.TheGame.Files.BetrayalJobs.GetByAddress(base.M.Read<long>(base.Address + 68));

	public BetrayalUpgrade Upgrade => ReadObjectAt<BetrayalUpgrade>(100);

	public string DialogueText => base.M.ReadStringU(base.M.Read<long>(base.Address + 166, new int[1] { 24 }));

	public List<int> Keys1 => ReadKeys(32L);

	public List<int> Keys2 => ReadKeys(84L);

	public List<int> Keys3 => ReadKeys(133L);

	private List<int> ReadKeys(long offset)
	{
		long num = base.M.Read<long>(base.Address + offset);
		List<int> list = new List<int>();
		if (num != 0L)
		{
			for (long num2 = 0L; num2 < 5; num2++)
			{
				list.Add(base.M.Read<int>(num + num2 * 8));
			}
		}
		return list;
	}

	public override string ToString()
	{
		return $"{Target?.Name}, {Job?.Name}, {Upgrade?.UpgradeName}, {DialogueText}";
	}
}
