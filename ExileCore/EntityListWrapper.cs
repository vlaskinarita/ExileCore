using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;

namespace ExileCore;

public class EntityListWrapper : IDisposable
{
	private readonly CoreSettings _settings;

	private readonly int coroutineTimeWait = 100;

	private readonly ConcurrentDictionary<uint, Entity> entityCache;

	private readonly GameController gameController;

	private readonly Queue<uint> keysForDelete = new Queue<uint>(24);

	private readonly Coroutine parallelUpdateDictionary;

	private readonly Stack<Entity> Simple = new Stack<Entity>(512);

	private readonly Coroutine updateEntity;

	private readonly EntityCollectSettingsContainer entityCollectSettingsContainer;

	private static EntityListWrapper _instance;

	private static readonly DebugInformation CollectEntitiesDebug = new DebugInformation("Collect Entities");

	public ICollection<Entity> Entities => entityCache.Values;

	public uint EntitiesVersion { get; }

	public Entity Player { get; private set; }

	public List<Entity> OnlyValidEntities { get; private set; } = new List<Entity>();


	public List<Entity> NotOnlyValidEntities { get; private set; } = new List<Entity>();


	public Dictionary<uint, Entity> NotValidDict { get; private set; } = new Dictionary<uint, Entity>();


	public Dictionary<EntityType, List<Entity>> ValidEntitiesByType { get; private set; } = PrepareEntityDictTemplate();


	public event Action<Entity> EntityAdded;

	public event Action<Entity> EntityAddedAny;

	public event Action<Entity> EntityIgnored;

	public event Action<Entity> EntityRemoved;

	public event EventHandler<Entity> PlayerUpdate;

	public EntityListWrapper(GameController gameController, CoreSettings settings, MultiThreadManager multiThreadManager)
	{
		EntityListWrapper entityListWrapper = this;
		_instance = this;
		this.gameController = gameController;
		_settings = settings;
		entityCache = new ConcurrentDictionary<uint, Entity>();
		gameController.Area.OnAreaChange += AreaChanged;
		EntitiesVersion = 0u;
		updateEntity = new Coroutine(RefreshState, new WaitTime(coroutineTimeWait), null, "Update Entity")
		{
			Priority = CoroutinePriority.High,
			SyncModWork = true
		};
		entityCollectSettingsContainer = new EntityCollectSettingsContainer
		{
			Simple = Simple,
			KeyForDelete = keysForDelete,
			EntityCache = entityCache,
			MultiThreadManager = multiThreadManager,
			ParseEntitiesInMultiThread = () => settings.PerformanceSettings.ParseEntitiesInMultiThread,
			EntitiesCount = () => gameController.IngameState.Data.EntitiesCount,
			EntitiesVersion = EntitiesVersion,
			CollectEntitiesInParallelWhenMoreThanX = settings.PerformanceSettings.CollectEntitiesInParallelWhenMoreThanX,
			DebugInformation = CollectEntitiesDebug
		};
		parallelUpdateDictionary = new Coroutine(Test(), null, "Collect entities")
		{
			SyncModWork = true
		};
		UpdateCondition(1000 / (int)settings.PerformanceSettings.EntitiesFps);
		settings.PerformanceSettings.EntitiesFps.OnValueChanged += OnEntitiesFpsValueChanged;
		PlayerUpdate += delegate(object? sender, Entity entity)
		{
			Entity.Player = entity;
		};
		IEnumerator Test()
		{
			while (true)
			{
				yield return gameController.IngameState.Data.EntityList.CollectEntities(entityListWrapper.entityCollectSettingsContainer);
				yield return new WaitTime(1000 / (int)settings.PerformanceSettings.EntitiesFps);
				entityListWrapper.parallelUpdateDictionary.UpdateTicks((uint)(entityListWrapper.parallelUpdateDictionary.Ticks + 1));
			}
		}
	}

	private void OnEntitiesFpsValueChanged(object sender, int i)
	{
		UpdateCondition(1000 / i);
	}

	public void StartWork()
	{
		Core.MainRunner.Run(updateEntity);
		Core.ParallelRunner.Run(parallelUpdateDictionary);
	}

	private void UpdateCondition(int coroutineTimeWait = 50)
	{
		parallelUpdateDictionary.UpdateCondtion(new WaitTime(coroutineTimeWait));
		updateEntity.UpdateCondtion(new WaitTime(coroutineTimeWait));
	}

	private void AreaChanged(AreaInstance area)
	{
		try
		{
			entityCollectSettingsContainer.Break = true;
			Entity localPlayer = gameController.Game.IngameState.Data.LocalPlayer;
			if (Player == null)
			{
				if (localPlayer != null && localPlayer.Path != null && localPlayer.Path.StartsWith("Meta"))
				{
					Player = localPlayer;
					Player.IsValid = true;
					this.PlayerUpdate?.Invoke(this, Player);
				}
			}
			else if (Player.Address != localPlayer.Address && localPlayer.Path.StartsWith("Meta"))
			{
				Player = localPlayer;
				Player.IsValid = true;
				this.PlayerUpdate?.Invoke(this, Player);
			}
			entityCache.Clear();
			OnlyValidEntities.Clear();
			NotOnlyValidEntities.Clear();
			foreach (KeyValuePair<EntityType, List<Entity>> item in ValidEntitiesByType)
			{
				item.Value.Clear();
			}
		}
		catch (Exception value)
		{
			DebugWindow.LogError($"{"EntityListWrapper"} -> {value}");
		}
	}

	private void UpdateEntityCollections()
	{
		while (keysForDelete.Count > 0)
		{
			uint key = keysForDelete.Dequeue();
			if (entityCache.TryGetValue(key, out var value))
			{
				this.EntityRemoved?.Invoke(value);
				entityCache.TryRemove(key, out var _);
			}
		}
		Dictionary<EntityType, List<Entity>> dictionary = PrepareEntityDictTemplate();
		List<Entity> list = new List<Entity>();
		List<Entity> list2 = new List<Entity>();
		Dictionary<uint, Entity> dictionary2 = new Dictionary<uint, Entity>();
		foreach (KeyValuePair<uint, Entity> item in entityCache)
		{
			Entity value3 = item.Value;
			if (value3.IsValid)
			{
				list.Add(value3);
				dictionary[value3.Type].Add(value3);
			}
			else
			{
				list2.Add(value3);
				dictionary2[value3.Id] = value3;
			}
		}
		ValidEntitiesByType = dictionary;
		OnlyValidEntities = list;
		NotOnlyValidEntities = list2;
		NotValidDict = dictionary2;
	}

	private static Dictionary<EntityType, List<Entity>> PrepareEntityDictTemplate()
	{
		Dictionary<EntityType, List<Entity>> dictionary = new Dictionary<EntityType, List<Entity>>();
		EntityType[] values = Enum.GetValues<EntityType>();
		foreach (EntityType key in values)
		{
			dictionary[key] = new List<Entity>();
		}
		return dictionary;
	}

	public void RefreshState()
	{
		if (gameController.Area.CurrentArea == null || entityCollectSettingsContainer.NeedUpdate || Player == null || !Player.IsValid)
		{
			return;
		}
		while (Simple.Count > 0)
		{
			Entity entity = Simple.Pop();
			if (entity == null)
			{
				DebugWindow.LogError("EntityListWrapper.RefreshState entity is null. (Very strange).");
				continue;
			}
			uint id = entity.Id;
			if (!entityCache.TryGetValue(id, out var _) && (id < int.MaxValue || (bool)_settings.PerformanceSettings.ParseServerEntities) && entity.Type != 0 && (entity.League != LeagueType.Legion || entity.Stats != null))
			{
				this.EntityAddedAny?.Invoke(entity);
				if (entity.Type >= EntityType.Monster)
				{
					this.EntityAdded?.Invoke(entity);
				}
				entityCache[id] = entity;
			}
		}
		UpdateEntityCollections();
		entityCollectSettingsContainer.NeedUpdate = true;
	}

	public static Entity GetEntityById(uint id)
	{
		if (!_instance.entityCache.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public string GetLabelForEntity(Entity entity)
	{
		HashSet<long> hashSet = new HashSet<long>();
		long num = gameController.Game.IngameState.EntityLabelMap;
		while (true)
		{
			hashSet.Add(num);
			if (gameController.Memory.Read<long>(num + 16) == entity.Address)
			{
				break;
			}
			num = gameController.Memory.Read<long>(num);
			if (hashSet.Contains(num) || num == 0L || num == -1)
			{
				return null;
			}
		}
		long num2 = gameController.Memory.Read<long>(num + 24, new int[1] { 448 });
		return gameController.Game.ReadObject<EntityLabel>(num2 + 744).Text;
	}

	public void Dispose()
	{
		updateEntity?.Done(force: true);
		parallelUpdateDictionary?.Done(force: true);
		_settings.PerformanceSettings.EntitiesFps.OnValueChanged -= OnEntitiesFpsValueChanged;
	}
}
