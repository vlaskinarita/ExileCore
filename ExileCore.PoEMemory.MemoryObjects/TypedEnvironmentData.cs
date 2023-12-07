using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.MemoryObjects;

public class TypedEnvironmentData<T> where T : unmanaged
{
	private readonly IMemory _m;

	private readonly long _environmentDataPtr;

	private readonly long _environmentPtr;

	private readonly int _startOffset;

	private readonly int _defaultStartOffset;

	private readonly int _count;

	public List<EnvironmentSettingValue<T>> Entries
	{
		get
		{
			List<DefaultEnvironmentSetting> second = (from x in _m.ReadMem<long>(_environmentDataPtr + _defaultStartOffset, _count)
				select (x != 0L) ? RemoteMemoryObject.GetObjectStatic<DefaultEnvironmentSetting>(x) : null).ToList();
			List<EnvironmentSettingValue<T>> list = _m.ReadStructsArray<EnvironmentSettingValue<T>>(_environmentPtr + _startOffset, _environmentPtr + _startOffset + _count * Unsafe.SizeOf<T>(), Unsafe.SizeOf<T>(), null);
			foreach (var item3 in list.Zip(second))
			{
				EnvironmentSettingValue<T> item = item3.First;
				DefaultEnvironmentSetting item2 = item3.Second;
				item.Default = item2;
			}
			return list;
		}
	}

	public TypedEnvironmentData(IMemory m, long environmentDataPtr, long environmentPtr, int startOffset, int defaultStartOffset, int count)
	{
		_m = m;
		_environmentDataPtr = environmentDataPtr;
		_environmentPtr = environmentPtr;
		_startOffset = startOffset;
		_defaultStartOffset = defaultStartOffset;
		_count = count;
	}
}
