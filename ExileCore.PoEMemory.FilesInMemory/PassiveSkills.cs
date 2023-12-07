using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class PassiveSkills : UniversalFileWrapper<PassiveSkill>
{
	private List<PassiveSkill> _EntriesList;

	private bool loaded;

	public Dictionary<int, PassiveSkill> PassiveSkillsDictionary { get; } = new Dictionary<int, PassiveSkill>();


	public new IList<PassiveSkill> EntriesList => _EntriesList ?? (_EntriesList = base.EntriesList.ToList());

	public PassiveSkills(IMemory m, Func<long> address)
		: base(m, address)
	{
	}

	public PassiveSkill GetPassiveSkillByPassiveId(int index)
	{
		CheckCache();
		if (!loaded)
		{
			foreach (PassiveSkill entries in EntriesList)
			{
				EntryAdded(entries.Address, entries);
			}
			loaded = true;
		}
		PassiveSkillsDictionary.TryGetValue(index, out var value);
		return value;
	}

	public PassiveSkill GetPassiveSkillById(string id)
	{
		return EntriesList.FirstOrDefault((PassiveSkill x) => x.Id == id);
	}

	protected new void EntryAdded(long addr, PassiveSkill entry)
	{
		PassiveSkillsDictionary.Add(entry.PassiveId, entry);
	}

	public new PassiveSkill GetByAddress(long address)
	{
		return base.GetByAddress(address);
	}
}
