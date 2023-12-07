using System;
using System.Collections.Generic;
using System.Numerics;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects;

public class Entity : RemoteMemoryObject
{
	internal static readonly int IdOffset = Extensions.GetOffset((EntityOffsets x) => x.Id);

	internal static readonly int FlagsOffset = Extensions.GetOffset((EntityOffsets x) => x.Flags);

	private readonly object _cacheComponentsLock = new object();

	private readonly Dictionary<Type, Component> _cacheComponents = new Dictionary<Type, Component>(4);

	private readonly CachedValue<bool> _hiddenCheckCache;

	private System.Numerics.Vector3 _boundsCenterPos = System.Numerics.Vector3.Zero;

	private Dictionary<string, long> _cacheComponents2;

	private float _distancePlayer = float.MaxValue;

	private EntityOffsets? _entityOffsets;

	private System.Numerics.Vector2 _gridPos = System.Numerics.Vector2.Zero;

	private long? _id;

	private uint? _inventoryId;

	private bool _isAlive;

	private bool _isDead;

	private CachedValue<bool> _isHostile;

	private bool _isOpened;

	private bool _isTargetable;

	private string _metadata;

	private string _path;

	private System.Numerics.Vector3 _pos = System.Numerics.Vector3.Zero;

	private MonsterRarity? _rarity;

	private string _renderName = "Empty";

	private Dictionary<GameStat, int> _stats;

	private readonly ValidCache<List<Buff>> buffCache;

	private bool isHidden;

	private readonly object locker = new object();

	private int pathReadErrorTimes;

	public static Entity Player { get; set; }

	private static Dictionary<string, string> ComponentsName { get; } = new Dictionary<string, string>();


	public float DistancePlayer
	{
		get
		{
			if (Player == null)
			{
				return _distancePlayer;
			}
			_ = IsValid;
			_distancePlayer = Player.GridPosNum.Distance(GridPosNum);
			return _distancePlayer;
		}
	}

	public EntityOffsets EntityOffsets
	{
		get
		{
			if (_entityOffsets.HasValue)
			{
				return _entityOffsets.Value;
			}
			if (base.Address != 0L)
			{
				_entityOffsets = base.M.Read<EntityOffsets>(base.Address);
			}
			if (_entityOffsets.HasValue)
			{
				return _entityOffsets.Value;
			}
			IsValid = false;
			return default(EntityOffsets);
		}
	}

	public EntityType Type { get; private set; }

	public LeagueType League { get; private set; }

	public StdVector ComponentList => EntityOffsets.ComponentList;

	public EntityFlags Flags => base.M.Read<EntityFlags>(base.Address + FlagsOffset);

	public bool IsHidden => _hiddenCheckCache.Value;

	public string Debug => $"{EntityOffsets.Head.MainObject:X} List: {EntityOffsets.ComponentList}";

	public uint Version { get; set; }

	public bool IsValid { get; set; }

	public bool IsAlive
	{
		get
		{
			if (!IsValid)
			{
				return _isAlive;
			}
			Life component = GetComponent<Life>();
			if (component == null || component.OwnerAddress != base.Address)
			{
				if (_distancePlayer < 70f)
				{
					_isAlive = false;
				}
				return _isAlive;
			}
			_isAlive = component.CurHP > 0;
			return _isAlive;
		}
	}

	[Obsolete]
	public SharpDX.Vector3 Pos => PosNum.ToSharpDx();

	public System.Numerics.Vector3 PosNum
	{
		get
		{
			if (!IsValid)
			{
				return _pos;
			}
			Render component = GetComponent<Render>();
			if (component != null)
			{
				_pos.X = component.X;
				_pos.Y = component.Y;
				_pos.Z = component.Z + component.BoundsNum.Z;
			}
			else
			{
				Positioned component2 = GetComponent<Positioned>();
				if (component2 != null)
				{
					_pos.X = component2.WorldX;
					_pos.Y = component2.WorldY;
				}
			}
			return _pos;
		}
	}

	[Obsolete]
	public SharpDX.Vector3 BoundsCenterPos => BoundsCenterPosNum.ToSharpDx();

	public System.Numerics.Vector3 BoundsCenterPosNum
	{
		get
		{
			if (!IsValid)
			{
				return _boundsCenterPos;
			}
			Render component = GetComponent<Render>();
			if (component == null)
			{
				return _boundsCenterPos;
			}
			_boundsCenterPos = component.InteractCenterNum;
			return _boundsCenterPos;
		}
	}

	[Obsolete]
	public SharpDX.Vector2 GridPos => GridPosNum.ToSharpDx();

	public System.Numerics.Vector2 GridPosNum
	{
		get
		{
			if (!IsValid)
			{
				return _gridPos;
			}
			Positioned component = GetComponent<Positioned>();
			if (component == null)
			{
				return _gridPos;
			}
			if (component.OwnerAddress != base.Address)
			{
				return _gridPos;
			}
			_gridPos = component.GridPosNum;
			return _gridPos;
		}
	}

	public string RenderName
	{
		get
		{
			if (!IsValid)
			{
				return _renderName;
			}
			Render component = GetComponent<Render>();
			if (component == null)
			{
				return _renderName;
			}
			_renderName = component.NameNoCache;
			return _renderName;
		}
	}

	public MonsterRarity Rarity
	{
		get
		{
			MonsterRarity? monsterRarity = (_rarity = _rarity ?? GetComponent<ObjectMagicProperties>()?.Rarity ?? MonsterRarity.White);
			return monsterRarity.Value;
		}
	}

	public bool IsOpened
	{
		get
		{
			if (!IsValid)
			{
				return _isOpened;
			}
			Chest component = GetComponent<Chest>();
			if (component == null)
			{
				return _isOpened;
			}
			Targetable component2 = GetComponent<Targetable>();
			if (component2 == null)
			{
				return _isOpened;
			}
			_isOpened = !component2.isTargetable || component.IsOpened;
			return _isOpened;
		}
	}

	public bool IsDead
	{
		get
		{
			if (!IsValid)
			{
				return _isDead;
			}
			_isDead = !_isAlive;
			return _isDead;
		}
	}

	public Dictionary<GameStat, int> Stats
	{
		get
		{
			if (!IsValid)
			{
				return _stats;
			}
			Stats stats = GetComponent<Stats>();
			if (stats == null)
			{
				return _stats;
			}
			if (stats.OwnerAddress != base.Address)
			{
				stats = GetComponentFromMemory<Stats>();
				if (stats.OwnerAddress != base.Address)
				{
					return _stats;
				}
			}
			Dictionary<GameStat, int> statDictionary = stats.StatDictionary;
			if (statDictionary.Count == 0 && (_stats == null || _stats.Count != 0))
			{
				return _stats;
			}
			_stats = statDictionary;
			return _stats;
		}
	}

	public bool IsTargetable
	{
		get
		{
			if (!IsValid)
			{
				if (_isTargetable && DistancePlayer < 200f)
				{
					_isTargetable = false;
				}
				return _isTargetable;
			}
			_isTargetable = GetComponent<Targetable>()?.isTargetable ?? false;
			return _isTargetable;
		}
	}

	public bool IsTransitioned => IsTransitionedHelper();

	public List<Buff> Buffs => buffCache.Value;

	private string CachePath { get; set; }

	public string Path
	{
		get
		{
			if (_path == null)
			{
				if (EntityOffsets.Head.MainObject == 0L)
				{
					if (CachePath == null)
					{
						IsValid = false;
						return null;
					}
					return CachePath;
				}
				PathEntityOffsets p = base.M.Read<PathEntityOffsets>(EntityOffsets.Head.MainObject);
				if (p.Path.Ptr == 0L)
				{
					if (CachePath == null)
					{
						IsValid = false;
						return null;
					}
					return CachePath;
				}
				_path = RemoteMemoryObject.Cache.StringCache.Read($"{p.Path.Ptr}{p.Length}", () => p.ToString(base.M));
				if (!_path.StartsWith("Metadata"))
				{
					_path = base.M.Read<PathEntityOffsets>(EntityOffsets.Head.MainObject).ToString(base.M);
					RemoteMemoryObject.Cache.StringCache.Remove($"{p.Path.Ptr}{p.Length}");
				}
				if (_path.Length > 0 && _path[0] != 'M')
				{
					pathReadErrorTimes++;
					IsValid = false;
					_path = null;
					if (pathReadErrorTimes > 10)
					{
						_path = "ERROR PATH";
						DebugWindow.LogError("Entity path error.");
					}
				}
				else
				{
					CachePath = _path;
				}
			}
			return _path;
		}
	}

	public string Metadata
	{
		get
		{
			if (_metadata == null && Path != null)
			{
				int num = Path.IndexOf("@", StringComparison.Ordinal);
				if (num == -1)
				{
					return Path;
				}
				_metadata = Path.Substring(0, num);
			}
			return _metadata;
		}
	}

	public uint Id
	{
		get
		{
			long valueOrDefault = _id.GetValueOrDefault();
			long num;
			if (!_id.HasValue)
			{
				valueOrDefault = base.M.Read<uint>(base.Address + IdOffset);
				_id = valueOrDefault;
				num = valueOrDefault;
			}
			else
			{
				num = valueOrDefault;
			}
			return (uint)num;
		}
	}

	public uint InventoryId
	{
		get
		{
			uint valueOrDefault = _inventoryId.GetValueOrDefault();
			if (!_inventoryId.HasValue)
			{
				valueOrDefault = base.M.Read<uint>(base.Address + 112);
				_inventoryId = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public Dictionary<string, long> CacheComp => _cacheComponents2 ?? (_cacheComponents2 = GetComponents());

	public bool IsHostile => _isHostile?.Value ?? (_isHostile = new TimeCache<bool>(() => (GetComponent<Positioned>()?.Reaction & 0x7F) != 1, 100L)).Value;

	private Dictionary<Type, object> PluginData { get; } = new Dictionary<Type, object>();


	public event EventHandler<Entity> OnUpdate;

	public Entity()
	{
		_hiddenCheckCache = new LatancyCache<bool>(delegate
		{
			if (IsValid)
			{
				isHidden = GetComponent<Buffs>()?.HasBuff("hidden_monster") ?? false;
			}
			return isHidden;
		}, 50);
		buffCache = this.ValidCache(() => GetComponent<Buffs>()?.BuffsList);
	}

	private bool IsTransitionedHelper()
	{
		byte? b = GetComponent<Transitionable>()?.Flag1;
		if (!b.HasValue)
		{
			return false;
		}
		return b.Value == 2;
	}

	public override string ToString()
	{
		return $"<{Type}> ({Rarity}) {Metadata}: ({base.Address:X})";
	}

	public float Distance(Entity entity)
	{
		return GridPosNum.Distance(entity.GridPosNum);
	}

	protected override void OnAddressChange()
	{
		_entityOffsets = base.M.Read<EntityOffsets>(base.Address);
		_inventoryId = null;
		_pos = System.Numerics.Vector3.Zero;
		_cacheComponents.Clear();
		_cacheComponents2 = null;
		if (Type == EntityType.Error)
		{
			Type = ParseType();
			if (Type != 0)
			{
				IsValid = true;
			}
		}
		this.OnUpdate?.Invoke(this, this);
	}

	public bool Check(uint entityId)
	{
		if (_id.HasValue && _id != entityId)
		{
			DebugWindow.LogMsg($"Was ID: {Id} New ID: {entityId} To Path: {Path}", 3f);
			_id = entityId;
			_path = null;
			_metadata = null;
			Type = ParseType();
		}
		if (Type != 0)
		{
			if (Type == EntityType.Effect || Type == EntityType.Daemon)
			{
				return true;
			}
			if (CacheComp != null && Id == entityId)
			{
				return CheckRarity();
			}
			return false;
		}
		return false;
	}

	private bool CheckRarity()
	{
		if (Rarity >= MonsterRarity.White)
		{
			return Rarity <= MonsterRarity.Unique;
		}
		return false;
	}

	public void UpdatePointer(long newAddress)
	{
		base.Address = newAddress;
	}

	private Dictionary<string, long> GetComponents()
	{
		lock (locker)
		{
			try
			{
				Dictionary<string, long> dictionary = new Dictionary<string, long>();
				long[] array = base.M.ReadStdVector<long>(ComponentList);
				ObjectHeaderOffsets objectHeaderOffsets = base.M.Read<ObjectHeaderOffsets>(EntityOffsets.EntityDetailsPtr);
				ComponentLookUpStruct componentLookUpStruct = base.M.Read<ComponentLookUpStruct>(objectHeaderOffsets.ComponentLookUpPtr);
				ComponentArrayStructure[] array2 = base.M.ReadMem<ComponentArrayStructure>(componentLookUpStruct.ComponentArray, ((int)componentLookUpStruct.Capacity + 1) / 8);
				for (int i = 0; i < array2.Length; i++)
				{
					ComponentArrayStructure componentArrayStructure = array2[i];
					if (componentArrayStructure.Flag0 != ComponentArrayStructure.InValidPointerFlagValue)
					{
						long strPtr8 = componentArrayStructure.Pointer0.NamePtr;
						string text = RemoteMemoryObject.Cache.StringCache.Read($"{"Entity"}{strPtr8}", () => base.M.ReadString(strPtr8));
						if (!string.IsNullOrEmpty(text) && !dictionary.ContainsKey(text))
						{
							dictionary.Add(text, array[componentArrayStructure.Pointer0.Index]);
						}
					}
					if (componentArrayStructure.Flag1 != ComponentArrayStructure.InValidPointerFlagValue)
					{
						long strPtr7 = componentArrayStructure.Pointer1.NamePtr;
						string text2 = RemoteMemoryObject.Cache.StringCache.Read($"{"Entity"}{strPtr7}", () => base.M.ReadString(strPtr7));
						if (!string.IsNullOrEmpty(text2) && !dictionary.ContainsKey(text2))
						{
							dictionary.Add(text2, array[componentArrayStructure.Pointer1.Index]);
						}
					}
					if (componentArrayStructure.Flag2 != ComponentArrayStructure.InValidPointerFlagValue)
					{
						long strPtr6 = componentArrayStructure.Pointer2.NamePtr;
						string text3 = RemoteMemoryObject.Cache.StringCache.Read($"{"Entity"}{strPtr6}", () => base.M.ReadString(strPtr6));
						if (!string.IsNullOrEmpty(text3) && !dictionary.ContainsKey(text3))
						{
							dictionary.Add(text3, array[componentArrayStructure.Pointer2.Index]);
						}
					}
					if (componentArrayStructure.Flag3 != ComponentArrayStructure.InValidPointerFlagValue)
					{
						long strPtr5 = componentArrayStructure.Pointer3.NamePtr;
						string text4 = RemoteMemoryObject.Cache.StringCache.Read($"{"Entity"}{strPtr5}", () => base.M.ReadString(strPtr5));
						if (!string.IsNullOrEmpty(text4) && !dictionary.ContainsKey(text4))
						{
							dictionary.Add(text4, array[componentArrayStructure.Pointer3.Index]);
						}
					}
					if (componentArrayStructure.Flag4 != ComponentArrayStructure.InValidPointerFlagValue)
					{
						long strPtr4 = componentArrayStructure.Pointer4.NamePtr;
						string text5 = RemoteMemoryObject.Cache.StringCache.Read($"{"Entity"}{strPtr4}", () => base.M.ReadString(strPtr4));
						if (!string.IsNullOrEmpty(text5) && !dictionary.ContainsKey(text5))
						{
							dictionary.Add(text5, array[componentArrayStructure.Pointer4.Index]);
						}
					}
					if (componentArrayStructure.Flag5 != ComponentArrayStructure.InValidPointerFlagValue)
					{
						long strPtr3 = componentArrayStructure.Pointer5.NamePtr;
						string text6 = RemoteMemoryObject.Cache.StringCache.Read($"{"Entity"}{strPtr3}", () => base.M.ReadString(strPtr3));
						if (!string.IsNullOrEmpty(text6) && !dictionary.ContainsKey(text6))
						{
							dictionary.Add(text6, array[componentArrayStructure.Pointer5.Index]);
						}
					}
					if (componentArrayStructure.Flag6 != ComponentArrayStructure.InValidPointerFlagValue)
					{
						long strPtr2 = componentArrayStructure.Pointer6.NamePtr;
						string text7 = RemoteMemoryObject.Cache.StringCache.Read($"{"Entity"}{strPtr2}", () => base.M.ReadString(strPtr2));
						if (!string.IsNullOrEmpty(text7) && !dictionary.ContainsKey(text7))
						{
							dictionary.Add(text7, array[componentArrayStructure.Pointer6.Index]);
						}
					}
					if (componentArrayStructure.Flag7 != ComponentArrayStructure.InValidPointerFlagValue)
					{
						long strPtr = componentArrayStructure.Pointer7.NamePtr;
						string text8 = RemoteMemoryObject.Cache.StringCache.Read($"{"Entity"}{strPtr}", () => base.M.ReadString(strPtr));
						if (!string.IsNullOrEmpty(text8) && !dictionary.ContainsKey(text8))
						{
							dictionary.Add(text8, array[componentArrayStructure.Pointer7.Index]);
						}
					}
				}
				return dictionary;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

	public bool HasComponent<T>() where T : Component, new()
	{
		if (CacheComp != null && CacheComp.TryGetValue(typeof(T).Name, out var value))
		{
			return value != 0;
		}
		return false;
	}

	public T GetComponentOld<T>() where T : Component, new()
	{
		lock (_cacheComponentsLock)
		{
			if (_cacheComponents.TryGetValue(typeof(T), out var value))
			{
				return (T)value;
			}
			if (CacheComp != null && CacheComp.TryGetValue(typeof(T).Name, out var value2))
			{
				T @object = GetObject<T>(value2);
				_cacheComponents[typeof(T)] = @object;
				return @object;
			}
			return null;
		}
	}

	public T GetComponent<T>() where T : Component, new()
	{
		lock (_cacheComponentsLock)
		{
			Component value = null;
			if (!_cacheComponents.TryGetValue(typeof(T), out value) && CacheComp != null && CacheComp.TryGetValue(typeof(T).Name, out var value2))
			{
				value = GetObject<T>(value2);
			}
			if (value != null && (value.OwnerAddress != base.Address || !(value is T)))
			{
				_cacheComponents.Remove(typeof(T));
				return null;
			}
			_cacheComponents[typeof(T)] = value;
			return (T)value;
		}
	}

	public bool TryGetComponent<T>(out T component) where T : Component, new()
	{
		component = GetComponent<T>();
		return component != null;
	}

	public bool CheckComponentForValid<T>() where T : Component, new()
	{
		if (GetComponent<T>().OwnerAddress != base.Address)
		{
			if (GetComponentFromMemory<T>().OwnerAddress == base.Address)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public T GetComponentFromMemory<T>() where T : Component, new()
	{
		lock (_cacheComponentsLock)
		{
			if (CacheComp.TryGetValue(typeof(T).Name, out var value))
			{
				T @object = GetObject<T>(value);
				_cacheComponents[typeof(T)] = @object;
				return @object;
			}
			return null;
		}
	}

	private EntityType ParseType()
	{
		string metadata = Metadata;
		if (metadata == null || metadata.Length == 0)
		{
			return EntityType.Error;
		}
		if (metadata.StartsWith("Metadata/Effects/", StringComparison.Ordinal))
		{
			return EntityType.Effect;
		}
		if (metadata.StartsWith("Metadata/Monsters/Daemon/", StringComparison.Ordinal))
		{
			return EntityType.Daemon;
		}
		if (Version != 0 && Id > int.MaxValue)
		{
			return EntityType.ServerObject;
		}
		if (HasComponent<WorldItem>())
		{
			return EntityType.WorldItem;
		}
		if (HasComponent<Monster>())
		{
			if (metadata.StartsWith("Metadata/Monsters/LeagueHeist/", StringComparison.Ordinal))
			{
				League = LeagueType.Heist;
			}
			if (metadata.StartsWith("Metadata/Monsters/LegionLeague/", StringComparison.Ordinal))
			{
				League = LeagueType.Legion;
			}
			if (metadata.StartsWith("Metadata/Monsters/LeagueAffliction/", StringComparison.Ordinal))
			{
				League = LeagueType.Delirium;
			}
			return EntityType.Monster;
		}
		if (HasComponent<Chest>())
		{
			if (metadata.StartsWith("Metadata/Chests/DelveChests", StringComparison.Ordinal))
			{
				League = LeagueType.Delve;
				return EntityType.Chest;
			}
			if (metadata.StartsWith("Metadata/Chests/Incursion", StringComparison.Ordinal))
			{
				League = LeagueType.Incursion;
				return EntityType.Chest;
			}
			if (metadata.StartsWith("Metadata/Chests/Legion", StringComparison.Ordinal))
			{
				League = LeagueType.Legion;
				return EntityType.Chest;
			}
			if (metadata.StartsWith("Metadata/Chests/LeagueHeist/HeistChest", StringComparison.Ordinal))
			{
				League = LeagueType.Heist;
				return EntityType.Chest;
			}
			return EntityType.Chest;
		}
		if (metadata.StartsWith("Metadata/NPC", StringComparison.Ordinal) && HasComponent<NPC>())
		{
			return EntityType.Npc;
		}
		if (HasComponent<Shrine>())
		{
			return EntityType.Shrine;
		}
		if (HasComponent<Player>())
		{
			return EntityType.Player;
		}
		if (metadata.StartsWith("Metadata/MiscellaneousObjects/Harvest", StringComparison.Ordinal) || metadata.StartsWith("Metadata/Terrain/Leagues/Harvest", StringComparison.Ordinal))
		{
			League = LeagueType.Harvest;
			return EntityType.MiscellaneousObjects;
		}
		if (HasComponent<MinimapIcon>())
		{
			if (metadata.Equals("Metadata/Terrain/Missions/Hideouts/Objects/HideoutCraftingBench", StringComparison.Ordinal))
			{
				return EntityType.CraftUnlock;
			}
			if (HasComponent<AreaTransition>())
			{
				return EntityType.AreaTransition;
			}
			if (metadata.EndsWith("Waypoint", StringComparison.Ordinal))
			{
				return EntityType.Waypoint;
			}
			if (HasComponent<Portal>())
			{
				return EntityType.TownPortal;
			}
			if (HasComponent<Monolith>())
			{
				return EntityType.Monolith;
			}
			if (HasComponent<Transitionable>() && metadata.StartsWith("Metadata/MiscellaneousObjects/Abyss"))
			{
				League = LeagueType.Abyss;
				return EntityType.MiscellaneousObjects;
			}
			if (metadata.Equals("Metadata/Terrain/Leagues/Legion/Objects/LegionInitiator", StringComparison.Ordinal))
			{
				return EntityType.LegionMonolith;
			}
			if (metadata.Equals("Metadata/MiscellaneousObjects/Stash", StringComparison.Ordinal))
			{
				return EntityType.Stash;
			}
			if (metadata.Equals("Metadata/MiscellaneousObjects/GuildStash", StringComparison.Ordinal))
			{
				return EntityType.GuildStash;
			}
			if (metadata.Equals("Metadata/MiscellaneousObjects/Delve/DelveCraftingBench", StringComparison.Ordinal))
			{
				return EntityType.DelveCraftingBench;
			}
			if (metadata.Equals("Metadata/MiscellaneousObjects/Breach/BreachObject", StringComparison.Ordinal))
			{
				return EntityType.Breach;
			}
			if (metadata.Equals("Metadata/Terrain/Leagues/Delve/Objects/DelveMineral"))
			{
				return EntityType.Resource;
			}
			return EntityType.IngameIcon;
		}
		if (HasComponent<Portal>())
		{
			return EntityType.Portal;
		}
		if (HasComponent<HideoutDoodad>())
		{
			return EntityType.HideoutDecoration;
		}
		if (HasComponent<Monolith>())
		{
			return EntityType.MiniMonolith;
		}
		if (HasComponent<ClientBetrayalChoice>())
		{
			return EntityType.BetrayalChoice;
		}
		if (HasComponent<RenderItem>())
		{
			return EntityType.Item;
		}
		if (metadata.StartsWith("Metadata/MiscellaneousObjects/Lights", StringComparison.Ordinal))
		{
			return EntityType.Light;
		}
		if (metadata.StartsWith("Metadata/Terrain/Labyrinth/Objects/Puzzle_Parts/Switch_Once", StringComparison.Ordinal))
		{
			return EntityType.DoorSwitch;
		}
		if (metadata.StartsWith("Metadata/Terrain", StringComparison.Ordinal))
		{
			return EntityType.Terrain;
		}
		if (metadata.StartsWith("Metadata/Pet", StringComparison.Ordinal))
		{
			return EntityType.Pet;
		}
		if (metadata.StartsWith("Metadata/MiscellaneousObjects/Door", StringComparison.Ordinal))
		{
			return EntityType.Door;
		}
		if (metadata.StartsWith("Metadata/MiscellaneousObjects", StringComparison.Ordinal))
		{
			return EntityType.MiscellaneousObjects;
		}
		if (HasComponent<TriggerableBlockage>() && !metadata.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssNodeSmall", StringComparison.OrdinalIgnoreCase) && !metadata.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssFinalNodeChest", StringComparison.OrdinalIgnoreCase) && !metadata.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssFinalNodeChest2", StringComparison.OrdinalIgnoreCase) && !metadata.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssFinalNodeChest3", StringComparison.OrdinalIgnoreCase) && !metadata.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssFinalNodeChest4", StringComparison.OrdinalIgnoreCase) && !metadata.Contains("AbyssFinalNodeSubArea", StringComparison.OrdinalIgnoreCase) && !metadata.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssNodeLarge", StringComparison.OrdinalIgnoreCase))
		{
			return EntityType.TriggerableBlockage;
		}
		return EntityType.None;
	}

	public T GetHudComponent<T>() where T : class
	{
		if (PluginData.TryGetValue(typeof(T), out var value))
		{
			return (T)value;
		}
		return null;
	}

	public void SetHudComponent<T>(T data)
	{
		lock (locker)
		{
			PluginData[typeof(T)] = data;
		}
	}
}
