using System.Collections.Generic;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ActiveSkillWrapper : RemoteMemoryObject
{
	public string InternalName => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string DisplayName => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public string Description => base.M.ReadStringU(base.M.Read<long>(base.Address + 16));

	public string SkillName => base.M.ReadStringU(base.M.Read<long>(base.Address + 24));

	public string Icon => base.M.ReadStringU(base.M.Read<long>(base.Address + 32));

	public List<int> CastTypes
	{
		get
		{
			List<int> list = new List<int>();
			int num = base.M.Read<int>(base.Address + 40);
			long num2 = base.M.Read<long>(base.Address + 48);
			for (int i = 0; i < num; i++)
			{
				list.Add(base.M.Read<int>(num2));
				num2 += 4;
			}
			return list;
		}
	}

	public List<int> SkillTypes
	{
		get
		{
			List<int> list = new List<int>();
			int num = base.M.Read<int>(base.Address + 56);
			long num2 = base.M.Read<long>(base.Address + 64);
			for (int i = 0; i < num; i++)
			{
				list.Add(base.M.Read<int>(num2));
				num2 += 4;
			}
			return list;
		}
	}

	public string LongDescription => base.M.ReadStringU(base.M.Read<long>(base.Address + 80));

	public string AmazonLink => base.M.ReadStringU(base.M.Read<long>(base.Address + 96));
}
