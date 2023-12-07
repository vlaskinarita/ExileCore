using System.Collections.Generic;
using ExileCore.Shared.Interfaces;
using GameOffsets;

namespace ExileCore.PoEMemory.FilesInMemory;

public static class Extensions
{
	public static List<T> ReadDat<T>(this DatArrayStruct array, IMemory memory, int itemSize) where T : unmanaged
	{
		return memory.ReadStructsArray<T>(array.ItemArrayPtr, array.ItemArrayPtr + array.Count * itemSize, itemSize);
	}

	public static List<long> ReadDatPtr(this DatArrayStruct array, IMemory memory)
	{
		return array.ReadDat<long>(memory, 8);
	}
}
