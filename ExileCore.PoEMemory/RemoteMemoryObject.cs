using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory;

public abstract class RemoteMemoryObject
{
	private long _address;

	public long Address
	{
		get
		{
			return _address;
		}
		protected set
		{
			if (_address != value)
			{
				_address = value;
				OnAddressChange();
			}
		}
	}

	public static Cache Cache => pCache;

	public IMemory M => pM;

	public TheGame TheGame => pTheGame;

	public static TheGame pTheGame { get; protected set; }

	protected static Cache pCache { get; set; }

	protected static IMemory pM { get; set; }

	protected virtual void OnAddressChange()
	{
	}

	public T ReadObjectAt<T>(int offset) where T : RemoteMemoryObject, new()
	{
		return ReadObject<T>(Address + offset);
	}

	public T ReadObject<T>(long addressPointer) where T : RemoteMemoryObject, new()
	{
		return GetObjectStatic<T>(M.Read<long>(addressPointer));
	}

	public T GetObjectAt<T>(int offset) where T : RemoteMemoryObject, new()
	{
		return GetObjectStatic<T>(Address + offset);
	}

	public T GetObjectAt<T>(long offset) where T : RemoteMemoryObject, new()
	{
		return GetObjectStatic<T>(Address + offset);
	}

	public T GetObject<T>(long address) where T : RemoteMemoryObject, new()
	{
		return GetObjectStatic<T>(address);
	}

	public static T GetObjectStatic<T>(long address) where T : RemoteMemoryObject, new()
	{
		return new T
		{
			Address = address
		};
	}

	public T GetObject<T>(IntPtr address) where T : RemoteMemoryObject, new()
	{
		return GetObjectStatic<T>(address.ToInt64());
	}

	public T AsObject<T>() where T : RemoteMemoryObject, new()
	{
		return GetObjectStatic<T>(Address);
	}

	public override bool Equals(object obj)
	{
		if (obj is RemoteMemoryObject remoteMemoryObject)
		{
			return remoteMemoryObject.Address == Address;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)Address + GetType().Name.GetHashCode();
	}

	public override string ToString()
	{
		return $"{Address:X}";
	}

	protected FrameCache<T> CreateStructFrameCache<T>() where T : unmanaged
	{
		return new FrameCache<T>(() => M.Read<T>(Address));
	}
}
