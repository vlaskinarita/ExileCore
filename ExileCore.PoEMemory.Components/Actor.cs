using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class Actor : Component
{
	public class ActionWrapper : RemoteMemoryObject
	{
		private readonly FrameCache<ActionWrapperOffsets> cacheValue;

		private Actor _actor;

		private ActionWrapperOffsets Struct => cacheValue.Value;

		public int DestinationX => Struct.Destination.X;

		public int DestinationY => Struct.Destination.Y;

		public Vector2i Destination => Struct.Destination;

		public Entity Target => GetObject<Entity>(Struct.Target);

		[Obsolete("Use Destination instead")]
		public Vector2i CastDestination => Destination;

		public ActorSkill Skill => GetObject<ActorSkill>(Struct.Skill).SetActor(_actor);

		public ActionWrapper()
		{
			cacheValue = new FrameCache<ActionWrapperOffsets>(() => base.M.Read<ActionWrapperOffsets>(base.Address));
		}

		public ActionWrapper SetActor(Actor actor)
		{
			_actor = actor;
			return this;
		}
	}

	private readonly CachedValue<ActorComponentOffsets> _cacheValue;

	private readonly CachedValue<AnimationController> _animationController;

	private ActorComponentOffsets Struct => _cacheValue.Value;

	public short ActionId
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return Struct.ActionId;
		}
	}

	public ActionFlags Action
	{
		get
		{
			if (base.Address == 0L)
			{
				return ActionFlags.None;
			}
			return (ActionFlags)Struct.ActionId;
		}
	}

	public bool isAttacking => (Action & ActionFlags.UsingAbility) > ActionFlags.None;

	public int AnimationId
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			return Struct.AnimationId;
		}
	}

	public AnimationE Animation
	{
		get
		{
			if (base.Address == 0L)
			{
				return AnimationE.Idle;
			}
			return (AnimationE)Struct.AnimationId;
		}
	}

	public AnimationController AnimationController => _animationController.Value;

	public ActionWrapper CurrentAction
	{
		get
		{
			if (Struct.ActionPtr <= 0)
			{
				return null;
			}
			return GetObject<ActionWrapper>(Struct.ActionPtr).SetActor(this);
		}
	}

	public bool isMoving
	{
		get
		{
			if ((Action & ActionFlags.Moving) > ActionFlags.None)
			{
				return true;
			}
			if (CurrentAction == null)
			{
				return false;
			}
			if (CurrentAction.Skill.Name == "Cyclone")
			{
				return true;
			}
			return false;
		}
	}

	public long DeployedObjectsCount => Struct.DeployedObjectArray.Size / 8;

	public List<DeployedObject> DeployedObjects
	{
		get
		{
			List<DeployedObject> list = new List<DeployedObject>();
			if ((Struct.DeployedObjectArray.Last - Struct.DeployedObjectArray.First) / 8 > 300)
			{
				return list;
			}
			for (long num = Struct.DeployedObjectArray.First; num < Struct.DeployedObjectArray.Last; num += 8)
			{
				list.Add(GetObject<DeployedObject>(num));
			}
			return list;
		}
	}

	public List<ActorSkill> ActorSkills
	{
		get
		{
			long first = Struct.ActorSkillsArray.First;
			long last = Struct.ActorSkillsArray.Last;
			first += 8;
			if ((last - first) / 16 > 50)
			{
				return new List<ActorSkill>();
			}
			List<ActorSkill> list = new List<ActorSkill>();
			for (long num = first; num < last; num += 16)
			{
				list.Add(ReadObject<ActorSkill>(num).SetActor(this));
			}
			return list;
		}
	}

	public List<ActorSkillCooldown> ActorSkillsCooldowns => base.M.ReadStructsArray<ActorSkillCooldown>(Struct.ActorSkillsCooldownArray.First, Struct.ActorSkillsCooldownArray.Last, 72, null);

	public List<ActorVaalSkill> ActorVaalSkills => base.M.ReadStructsArray<ActorVaalSkill>(Struct.ActorVaalSkills.First, Struct.ActorVaalSkills.Last, 32, null);

	[Obsolete("Use ActorSkillsCooldowns")]
	public IEnumerable<long> SkillUiStateOffsets => ActorSkillsCooldowns.Select((ActorSkillCooldown x) => x.Address);

	public Actor()
	{
		_cacheValue = new FrameCache<ActorComponentOffsets>(() => base.M.Read<ActorComponentOffsets>(base.Address));
		_animationController = KeyTrackingCache.Create(() => GetObject<AnimationController>(Struct.AnimationControllerPtr), () => Struct.AnimationControllerPtr);
	}
}
