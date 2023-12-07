using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;
using GameOffsets.Native;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ServerInventory : RemoteMemoryObject
{
	public class HashNode : RemoteMemoryObject
	{
		private readonly FrameCache<NativeHashNode> frameCache;

		private NativeHashNode NativeHashNode => frameCache.Value;

		public HashNode Previous => GetObject<HashNode>(NativeHashNode.Previous);

		public HashNode Root => GetObject<HashNode>(NativeHashNode.Root);

		public HashNode Next => GetObject<HashNode>(NativeHashNode.Next);

		public bool IsNull => NativeHashNode.IsNull != 0;

		public int Key => NativeHashNode.Key;

		public InventSlotItem Value1 => GetObject<InventSlotItem>(NativeHashNode.Value1);

		public HashNode()
		{
			frameCache = new FrameCache<NativeHashNode>(() => base.M.Read<NativeHashNode>(base.Address));
		}
	}

	public class InventSlotItem : RemoteMemoryObject
	{
		private struct ItemMinMaxLocation
		{
			private int XMin { get; set; }

			private int YMin { get; set; }

			private int XMax { get; set; }

			private int YMax { get; set; }

			[Obsolete]
			public SharpDX.Vector2 InventoryPosition => new SharpDX.Vector2(XMin, YMin);

			public System.Numerics.Vector2 InventoryPositionNum => new System.Numerics.Vector2(XMin, YMin);

			public RectangleF GetItemRect(float invStartX, float invStartY, float cellsize)
			{
				return new RectangleF(invStartX + cellsize * (float)XMin, invStartY + cellsize * (float)YMin, (float)(XMax - XMin) * cellsize, (float)(YMax - YMin) * cellsize);
			}

			public override string ToString()
			{
				return $"({XMin}, {YMin}, {XMax}, {YMax})";
			}
		}

		[Obsolete]
		public SharpDX.Vector2 InventoryPosition => Location.InventoryPosition;

		public System.Numerics.Vector2 InventoryPositionNum => Location.InventoryPositionNum;

		private ItemMinMaxLocation Location => base.M.Read<ItemMinMaxLocation>(base.Address + 8);

		public Entity Item => ReadObject<Entity>(base.Address);

		public int PosX => base.M.Read<int>(base.Address + 8);

		public int PosY => base.M.Read<int>(base.Address + 12);

		public int SizeX => base.M.Read<int>(base.Address + 16) - PosX;

		public int SizeY => base.M.Read<int>(base.Address + 20) - PosY;

		private RectangleF ClientRect => GetClientRect();

		public RectangleF GetClientRect()
		{
			RectangleF clientRect = base.TheGame.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].GetClientRect();
			float cellsize = clientRect.Width / 12f;
			return Location.GetItemRect(clientRect.X, clientRect.Y, cellsize);
		}

		public override string ToString()
		{
			return $"InventSlotItem: {Location}, Item: {Item}";
		}
	}

	private readonly CachedValue<ServerInventoryOffsets> cachedValue;

	private const int HashReadLimit = 500;

	private ServerInventoryOffsets Struct => cachedValue.Value;

	public InventoryTypeE InventType => (InventoryTypeE)Struct.InventType;

	public InventorySlotE InventSlot => (InventorySlotE)Struct.InventSlot;

	public int Columns => Struct.Columns;

	public int Rows => Struct.Rows;

	public bool IsRequested => Struct.IsRequested == 1;

	public long ItemCount => Struct.ItemCount;

	[Obsolete]
	public long CountItems => ItemCount;

	[Obsolete]
	public int TotalItemsCounts => (int)ItemCount;

	public int ServerRequestCounter => Struct.ServerRequestCounter;

	public IList<InventSlotItem> InventorySlotItems => ReadHashMap(Struct.InventorySlotItemsPtr, 500).Values.ToList();

	public long Hash => Struct.Hash;

	public IList<Entity> Items => InventorySlotItems.Select((InventSlotItem x) => x.Item).ToList();

	public InventSlotItem this[int x, int y]
	{
		get
		{
			long inventoryItemsPtr = Struct.InventoryItemsPtr;
			long num = base.M.Read<long>(inventoryItemsPtr + (x + y * Columns) * 8);
			if (num <= 0)
			{
				return null;
			}
			return GetObject<InventSlotItem>(num);
		}
	}

	public ServerInventory()
	{
		cachedValue = new FrameCache<ServerInventoryOffsets>(() => base.M.Read<ServerInventoryOffsets>(base.Address));
	}

	public Dictionary<int, InventSlotItem> ReadHashMap(long pointer, int limitMax)
	{
		Dictionary<int, InventSlotItem> dictionary = new Dictionary<int, InventSlotItem>();
		Stack<HashNode> stack = new Stack<HashNode>();
		HashNode root = GetObject<HashNode>(pointer).Root;
		stack.Push(root);
		while (stack.Count != 0)
		{
			HashNode hashNode = stack.Pop();
			if (!hashNode.IsNull)
			{
				dictionary[hashNode.Key] = hashNode.Value1;
			}
			HashNode previous = hashNode.Previous;
			if (!previous.IsNull)
			{
				stack.Push(previous);
			}
			HashNode next = hashNode.Next;
			if (!next.IsNull)
			{
				stack.Push(next);
			}
			if (limitMax-- < 0)
			{
				DebugWindow.LogError("Fixed possible memory leak (ServerInventory.ReadHashMap)");
				break;
			}
		}
		return dictionary;
	}
}
