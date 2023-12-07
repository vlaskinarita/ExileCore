using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class Sockets : Component
{
	public class SocketedGem
	{
		public Entity GemEntity;

		public int SocketIndex;
	}

	public class Socket
	{
		public SocketColor SocketColor;

		public int LinkGroup;

		public int SocketedGemIndex = -1;

		public Entity SocketedGemEntity;

		public long GemAddress = -1L;

		public Socket(SocketColor socketColor, int linkGroup, int socketIndex, Entity gemEntity)
		{
			SocketColor = socketColor;
			LinkGroup = linkGroup;
			SocketedGemEntity = gemEntity;
			if (gemEntity != null)
			{
				SocketedGemIndex = socketIndex;
				GemAddress = SocketedGemEntity.Address;
			}
		}

		public override string ToString()
		{
			bool num = SocketedGemEntity != null && SocketedGemEntity.HasComponent<SkillGem>();
			string value = "SocketedItem(NONE)";
			if (num)
			{
				if (SocketedGemEntity != null)
				{
					value = $"SocketedItem({SocketedGemEntity.GetComponent<Base>().Name}({SocketedGemEntity.GetComponent<SkillGem>().Level}))";
				}
			}
			else if (SocketedGemEntity != null)
			{
				value = "SocketedItem(" + SocketedGemEntity.GetComponent<Base>().Name + ")";
			}
			string value2 = ((SocketedGemEntity != null) ? $", SocketedItemAddress({GemAddress:X})" : ", SocketedItemAddress(NONE)");
			return $"{value}, SocketColor({SocketColor}), LinkGroup({LinkGroup}), SocketedItemIndex({SocketedGemIndex}){value2}";
		}
	}

	private readonly CachedValue<SocketsComponentOffsets> _cachedValue;

	public int LargestLinkSize
	{
		get
		{
			if (base.Address == 0L)
			{
				return 0;
			}
			StdVector linkSizes = _cachedValue.Value.LinkSizes;
			if (linkSizes.ElementCount<byte>() > 6)
			{
				return 0;
			}
			return (int)base.M.ReadStdVector<byte>(linkSizes).Max();
		}
	}

	public List<int[]> Links
	{
		get
		{
			if (base.Address == 0L)
			{
				return new List<int[]>();
			}
			StdVector linkSizes = _cachedValue.Value.LinkSizes;
			if (linkSizes.ElementCount<byte>() > 6)
			{
				return new List<int[]>();
			}
			byte[] array = base.M.ReadStdVector<byte>(linkSizes);
			List<int> socketList = SocketList;
			List<int[]> list = new List<int[]>();
			int num = 0;
			byte[] array2 = array;
			foreach (int num2 in array2)
			{
				int num3 = num + num2;
				if (num3 > socketList.Count)
				{
					return list;
				}
				int[] item = socketList.Take(num..num3).ToArray();
				list.Add(item);
				num = num3;
			}
			return list;
		}
	}

	public List<int> SocketList
	{
		get
		{
			SocketColorList sockets = _cachedValue.Value.Sockets;
			return new int[6] { sockets.Socket1Color, sockets.Socket2Color, sockets.Socket3Color, sockets.Socket4Color, sockets.Socket5Color, sockets.Socket6Color }.Where((int x) => x >= 1 && x <= 6).ToList();
		}
	}

	public int NumberOfSockets => SocketList.Count;

	public bool IsRGB
	{
		get
		{
			if (base.Address != 0L)
			{
				return Links.Any((int[] current) => current.Length >= 3 && current.Contains(1) && current.Contains(2) && current.Contains(3));
			}
			return false;
		}
	}

	[Obsolete("Use GetSocketInfo instead")]
	public List<string> SocketGroup
	{
		get
		{
			List<string> list = new List<string>();
			foreach (int[] link in Links)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int[] array = link;
				for (int i = 0; i < array.Length; i++)
				{
					switch (array[i])
					{
					case 1:
						stringBuilder.Append("R");
						break;
					case 2:
						stringBuilder.Append("G");
						break;
					case 3:
						stringBuilder.Append("B");
						break;
					case 4:
						stringBuilder.Append("W");
						break;
					case 5:
						stringBuilder.Append('A');
						break;
					case 6:
						stringBuilder.Append("O");
						break;
					}
				}
				list.Add(stringBuilder.ToString());
			}
			return list;
		}
	}

	public List<SocketedGem> SocketedGems
	{
		get
		{
			SocketedGemList socketedGems = _cachedValue.Value.SocketedGems;
			return (from x in new long[6] { socketedGems.Socket1GemPtr, socketedGems.Socket2GemPtr, socketedGems.Socket3GemPtr, socketedGems.Socket4GemPtr, socketedGems.Socket5GemPtr, socketedGems.Socket6GemPtr }.Select((long pointer, int index) => (pointer, index))
				where x.pointer != 0
				select new SocketedGem
				{
					SocketIndex = x.index,
					GemEntity = GetObject<Entity>(x.pointer)
				}).ToList();
		}
	}

	public List<List<Socket>> SocketInfoByLinkGroup
	{
		get
		{
			List<SocketedGem> socketedGems = SocketedGems;
			List<int[]> links = Links;
			List<List<Socket>> list = new List<List<Socket>>();
			int index = 0;
			int num = 0;
			foreach (int[] item in links)
			{
				List<Socket> list2 = new List<Socket>();
				int[] array = item;
				foreach (int socketColor in array)
				{
					SocketedGem socketedGem = socketedGems.FirstOrDefault((SocketedGem gem) => gem.SocketIndex == index);
					list2.Add(new Socket((SocketColor)socketColor, num, socketedGem?.SocketIndex ?? (-1), socketedGem?.GemEntity));
					index++;
				}
				num++;
				list.Add(list2);
			}
			return list;
		}
	}

	public List<Socket> SocketInfo => GetSocketInfo();

	public Sockets()
	{
		_cachedValue = CreateStructFrameCache<SocketsComponentOffsets>();
	}

	public List<Socket> GetSocketInfo()
	{
		List<SocketedGem> socketedGems = SocketedGems;
		List<int[]> links = Links;
		List<Socket> list = new List<Socket>();
		int index = 0;
		int num = 0;
		foreach (int[] item in links)
		{
			foreach (int socketColor in item)
			{
				SocketedGem socketedGem = socketedGems.FirstOrDefault((SocketedGem gem) => gem.SocketIndex == index);
				list.Add(new Socket((SocketColor)socketColor, num, socketedGem?.SocketIndex ?? (-1), socketedGem?.GemEntity));
				index++;
			}
			num++;
		}
		return list;
	}
}
