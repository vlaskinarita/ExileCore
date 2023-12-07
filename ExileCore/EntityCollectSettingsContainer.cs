using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Nodes;

namespace ExileCore;

public class EntityCollectSettingsContainer
{
	public Stack<Entity> Simple { get; set; }

	public Queue<uint> KeyForDelete { get; set; }

	public ConcurrentDictionary<uint, Entity> EntityCache { get; set; }

	public MultiThreadManager MultiThreadManager { get; set; }

	public Func<long> EntitiesCount { get; set; }

	public uint EntitiesVersion { get; set; }

	public bool NeedUpdate { get; set; } = true;


	public ToggleNode CollectEntitiesInParallelWhenMoreThanX { get; set; }

	public DebugInformation DebugInformation { get; set; }

	public bool Break { get; set; }

	public Func<bool> ParseEntitiesInMultiThread { get; set; }
}
