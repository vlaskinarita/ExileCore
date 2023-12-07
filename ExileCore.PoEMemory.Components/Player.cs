using System;
using System.Collections;
using System.Collections.Generic;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Enums;
using SharpDX;

namespace ExileCore.PoEMemory.Components;

public class Player : Component
{
	public class TrialState
	{
		public LabyrinthTrial TrialArea { get; internal set; }

		public string TrialAreaId { get; internal set; }

		public bool IsCompleted { get; internal set; }

		public string AreaAddr => TrialArea.Address.ToString("x");

		public override string ToString()
		{
			return $"Completed: {IsCompleted}, Trial {TrialArea.Area.Name}, AreaId: {TrialArea.Id}";
		}
	}

	private const int LevelOffset = 428;

	private const int AttributeOffset = 412;

	public string PlayerName => NativeStringReader.ReadString(base.Address + 360, base.M);

	public uint XP
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0u;
			}
			return base.M.Read<uint>(base.Address + 396);
		}
	}

	public int Strength
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 412);
		}
	}

	public int Dexterity
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 412 + 4);
		}
	}

	public int Intelligence
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return base.M.Read<int>(base.Address + 412 + 8);
		}
	}

	public int Level
	{
		get
		{
			if (base.Address == 0L)
			{
				return 1;
			}
			return base.M.Read<byte>(base.Address + 428);
		}
	}

	public int AllocatedLootId
	{
		get
		{
			if (base.Address == 0L)
			{
				return 1;
			}
			return base.M.Read<byte>(base.Address + 404);
		}
	}

	public int HideoutLevel => base.M.Read<byte>(base.Address + 910);

	public HideoutWrapper Hideout => ReadObject<HideoutWrapper>(base.Address + 488);

	public PantheonGod PantheonMinor => (PantheonGod)base.M.Read<byte>(base.Address + 420);

	public PantheonGod PantheonMajor => (PantheonGod)base.M.Read<byte>(base.Address + 421);

	[HideInReflection]
	private BitArray TrialPassStates => new BitArray(base.M.ReadBytes(base.Address + 692, 36));

	public IList<TrialState> TrialStates
	{
		get
		{
			List<TrialState> list = new List<TrialState>();
			BitArray trialPassStates = TrialPassStates;
			string[] labyrinthTrialAreaIds = LabyrinthTrials.LabyrinthTrialAreaIds;
			foreach (string text in labyrinthTrialAreaIds)
			{
				LabyrinthTrial labyrinthTrialByAreaId = base.TheGame.Files.LabyrinthTrials.GetLabyrinthTrialByAreaId(text);
				list.Add(new TrialState
				{
					TrialAreaId = text,
					TrialArea = labyrinthTrialByAreaId,
					IsCompleted = trialPassStates.Get(labyrinthTrialByAreaId.Id - 1)
				});
			}
			return list;
		}
	}

	private IList<PassiveSkill> AllocatedPassivesM()
	{
		List<PassiveSkill> list = new List<PassiveSkill>();
		foreach (ushort passiveSkillId in base.TheGame.IngameState.ServerData.PassiveSkillIds)
		{
			PassiveSkill passiveSkillByPassiveId = base.TheGame.Files.PassiveSkills.GetPassiveSkillByPassiveId(passiveSkillId);
			if (passiveSkillByPassiveId == null)
			{
				DebugWindow.LogMsg($"Can't find passive with id: {passiveSkillId}", 10f, Color.Red);
			}
			else
			{
				list.Add(passiveSkillByPassiveId);
			}
		}
		return list;
	}

	public bool IsTrialCompleted(string trialId)
	{
		LabyrinthTrial labyrinthTrialByAreaId = base.TheGame.Files.LabyrinthTrials.GetLabyrinthTrialByAreaId(trialId);
		if (labyrinthTrialByAreaId == null)
		{
			throw new ArgumentException("Trial with id '" + trialId + "' is not found. Use WorldArea.Id or LabyrinthTrials.LabyrinthTrialAreaIds[]");
		}
		return TrialPassStates.Get(labyrinthTrialByAreaId.Id - 1);
	}

	public bool IsTrialCompleted(LabyrinthTrial trialWrapper)
	{
		if (trialWrapper == null)
		{
			throw new ArgumentException("Argument trialWrapper should not be null");
		}
		return TrialPassStates.Get(trialWrapper.Id - 1);
	}

	public bool IsTrialCompleted(WorldArea area)
	{
		if (area == null)
		{
			throw new ArgumentException("Argument area should not be null");
		}
		LabyrinthTrial labyrinthTrialByArea = base.TheGame.Files.LabyrinthTrials.GetLabyrinthTrialByArea(area);
		if (labyrinthTrialByArea == null)
		{
			throw new ArgumentException("Can't find trial wrapper for area '" + area.Name + "' (seems not a trial area).");
		}
		return TrialPassStates.Get(labyrinthTrialByArea.Id - 1);
	}
}
