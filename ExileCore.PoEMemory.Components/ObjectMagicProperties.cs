using System;
using System.Collections.Generic;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;
using SharpDX;

namespace ExileCore.PoEMemory.Components;

public class ObjectMagicProperties : Component
{
	private readonly CachedValue<ObjectMagicPropertiesOffsets> _CachedValue;

	private long _ModsHash;

	private readonly List<string> _ModNamesList = new List<string>();

	private const int MOD_RECORDS_OFFSET = 24;

	private const int MOD_RECORD_SIZE = 56;

	private const int MOD_RECORD_KEY_OFFSET = 16;

	public ObjectMagicPropertiesOffsets ObjectMagicPropertiesOffsets => _CachedValue.Value;

	public MonsterRarity Rarity
	{
		get
		{
			if (base.Address != 0L)
			{
				return (MonsterRarity)ObjectMagicPropertiesOffsets.Rarity;
			}
			return MonsterRarity.Error;
		}
	}

	public long ModsHash => ObjectMagicPropertiesOffsets.Mods.GetHashCode();

	public List<string> Mods
	{
		get
		{
			if (base.Address == 0L)
			{
				return null;
			}
			if (_ModsHash == ModsHash)
			{
				return _ModNamesList;
			}
			long first = ObjectMagicPropertiesOffsets.Mods.First;
			long last = ObjectMagicPropertiesOffsets.Mods.Last;
			long num = ObjectMagicPropertiesOffsets.Mods.First + 14336;
			if (first == 0L || last == 0L || last < first)
			{
				return new List<string>();
			}
			last = Math.Min(last, num);
			for (long num2 = first + 24; num2 < last; num2 += 56)
			{
				long read = base.M.Read<long>(num2 + 16, new int[1]);
				string item = RemoteMemoryObject.Cache.StringCache.Read($"{"ObjectMagicProperties"}{read}", () => base.M.ReadStringU(read));
				_ModNamesList.Add(item);
			}
			if (first == num)
			{
				DebugWindow.LogMsg("ObjectMagicProperties read mods error address", 2f, Color.OrangeRed);
			}
			_ModsHash = ModsHash;
			return _ModNamesList;
		}
	}

	public ObjectMagicProperties()
	{
		_CachedValue = new FrameCache<ObjectMagicPropertiesOffsets>(() => base.M.Read<ObjectMagicPropertiesOffsets>(base.Address));
	}
}
