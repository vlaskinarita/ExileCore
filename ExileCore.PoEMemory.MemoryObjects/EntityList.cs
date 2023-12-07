using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using GameOffsets;
using JM.LinqFaster;

namespace ExileCore.PoEMemory.MemoryObjects;

public class EntityList : RemoteMemoryObject
{
	private readonly WaitTime collectEntities = new WaitTime(1);

	private readonly List<long> hashAddresses = new List<long>(1000);

	private readonly HashSet<long> hashSet = new HashSet<long>(256);

	private readonly object locker = new object();

	private readonly Queue<long> queue = new Queue<long>(256);

	private readonly HashSet<long> StoreIds = new HashSet<long>(256);

	private readonly Stopwatch sw = Stopwatch.StartNew();

	public int EntitiesProcessed { get; private set; }

	public IEnumerator CollectEntities(EntityCollectSettingsContainer container)
	{
		if (base.Address == 0L)
		{
			DebugWindow.LogError("EntityList -> Address is 0;");
			yield return new WaitTime(100);
		}
		while (!container.NeedUpdate)
		{
			yield return collectEntities;
		}
		sw.Restart();
		long num = container.EntitiesCount();
		double num2 = 0.0;
		long num3 = base.M.Read<long>(base.Address + 8);
		hashAddresses.Clear();
		hashSet.Clear();
		StoreIds.Clear();
		queue.Enqueue(num3);
		EntityListOffsets entityListOffsets = base.M.Read<EntityListOffsets>(num3);
		queue.Enqueue(entityListOffsets.FirstAddr);
		queue.Enqueue(entityListOffsets.SecondAddr);
		int num4 = 0;
		while (queue.Count > 0 && num4 < 10000)
		{
			try
			{
				num4++;
				long num5 = queue.Dequeue();
				if (hashSet.Contains(num5))
				{
					continue;
				}
				hashSet.Add(num5);
				if (num5 != num3 && num5 != 0L)
				{
					long entity = entityListOffsets.Entity;
					if (entity > 4096 && entity < 139637976727552L)
					{
						hashAddresses.Add(entity);
					}
					entityListOffsets = base.M.Read<EntityListOffsets>(num5);
					queue.Enqueue(entityListOffsets.FirstAddr);
					queue.Enqueue(entityListOffsets.SecondAddr);
				}
			}
			catch (Exception value)
			{
				DebugWindow.LogError($"Entitylist while loop: {value}");
			}
		}
		EntitiesProcessed = hashAddresses.Count;
		if (num > 0 && (float)(EntitiesProcessed / num) > 1.5f)
		{
			DebugWindow.LogError($"Something wrong we parse {EntitiesProcessed} when expect {num}");
			base.TheGame.IngameState.UpdateData();
		}
		if (container.ParseEntitiesInMultiThread() && (bool)container.CollectEntitiesInParallelWhenMoreThanX && container.MultiThreadManager != null && EntitiesProcessed / container.MultiThreadManager.ThreadsCount >= 100)
		{
			int hashAddressesCount = hashAddresses.Count / container.MultiThreadManager.ThreadsCount;
			List<Job> jobs = new List<Job>(container.MultiThreadManager.ThreadsCount);
			for (int i = 1; i <= container.MultiThreadManager.ThreadsCount; i++)
			{
				int i2 = i;
				Job item = container.MultiThreadManager.AddJob(delegate
				{
					try
					{
						int num6 = ((i != container.MultiThreadManager.ThreadsCount) ? (i2 * hashAddressesCount) : hashAddresses.Count);
						int num7 = (i2 - 1) * hashAddressesCount;
						Span<uint> span = stackalloc uint[num6 - num7];
						int num8 = 0;
						for (int j = num7; j < num6; j++)
						{
							long addrEntity = hashAddresses[j];
							span[num8] = ParseEntity(addrEntity, container.EntityCache, container.EntitiesVersion, container.Simple);
							num8++;
						}
						lock (locker)
						{
							Span<uint> span2 = span;
							for (int k = 0; k < span2.Length; k++)
							{
								uint num9 = span2[k];
								StoreIds.Add(num9);
							}
						}
					}
					catch (Exception value3)
					{
						DebugWindow.LogError($"{value3}");
					}
				}, $"EntityCollection {i}");
				jobs.Add(item);
			}
			while (!jobs.AllF((Job x) => x.IsCompleted))
			{
				sw.Stop();
				yield return collectEntities;
				sw.Start();
			}
			num2 = jobs.SumF((Job x) => x.ElapsedMs);
		}
		else
		{
			foreach (long hashAddress in hashAddresses)
			{
				StoreIds.Add(ParseEntity(hashAddress, container.EntityCache, container.EntitiesVersion, container.Simple));
			}
		}
		if (container.Break)
		{
			container.Break = false;
			container.EntitiesVersion++;
			container.NeedUpdate = false;
			container.DebugInformation.Tick = sw.Elapsed.TotalMilliseconds;
			yield break;
		}
		foreach (KeyValuePair<uint, Entity> item2 in container.EntityCache)
		{
			Entity value2 = item2.Value;
			if (StoreIds.Contains(item2.Key))
			{
				value2.IsValid = true;
				continue;
			}
			value2.IsValid = false;
			float distancePlayer = value2.DistancePlayer;
			if (distancePlayer < 100f)
			{
				if (distancePlayer < 75f)
				{
					if (value2.Type != EntityType.Chest || value2.League != LeagueType.Delve)
					{
						container.KeyForDelete.Enqueue(item2.Key);
						continue;
					}
					if (distancePlayer < 30f)
					{
						container.KeyForDelete.Enqueue(item2.Key);
						continue;
					}
				}
				if (value2.Type == EntityType.Monster && value2.IsAlive)
				{
					container.KeyForDelete.Enqueue(item2.Key);
					continue;
				}
			}
			if (distancePlayer > 300f && item2.Value.Metadata.Equals("Metadata/Monsters/Totems/HeiTikiSextant", StringComparison.Ordinal))
			{
				container.KeyForDelete.Enqueue(item2.Key);
			}
			else if (value2.Type < EntityType.Monster)
			{
				container.KeyForDelete.Enqueue(item2.Key);
			}
			else if (distancePlayer > 1000000f || item2.Value.GridPosNum == Vector2.Zero)
			{
				container.KeyForDelete.Enqueue(item2.Key);
			}
		}
		container.EntitiesVersion++;
		container.NeedUpdate = false;
		container.DebugInformation.Tick = sw.Elapsed.TotalMilliseconds + num2;
	}

	private uint ParseEntity(long addrEntity, IReadOnlyDictionary<uint, Entity> entityCache, uint entitiesVersion, Stack<Entity> result)
	{
		uint num = base.M.Read<uint>(addrEntity + Entity.IdOffset);
		if (num == 0)
		{
			return 0u;
		}
		if (entityCache.TryGetValue(num, out var value))
		{
			if (value.Address != addrEntity)
			{
				value.UpdatePointer(addrEntity);
				if (value.Check(num))
				{
					value.Version = entitiesVersion;
					value.IsValid = true;
				}
			}
			else
			{
				value.Version = entitiesVersion;
				value.IsValid = true;
			}
		}
		else
		{
			value = GetObject<Entity>(addrEntity);
			if (value.Check(num))
			{
				value.Version = entitiesVersion;
				result.Push(value);
				value.IsValid = true;
			}
		}
		return num;
	}
}
