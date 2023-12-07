using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.MemoryObjects;

public class NativeStringReader
{
	public static string ReadString(long address, IMemory M)
	{
		return ReadString(address, M, 512);
	}

	public static string ReadString(long address, IMemory M, int lengthBytes)
	{
		uint num = M.Read<uint>(address + 24);
		if (8 <= num)
		{
			long addr = M.Read<long>(address);
			return M.ReadStringU(addr, lengthBytes);
		}
		return M.ReadStringU(address, lengthBytes);
	}

	public static string ReadStringLong(long address, IMemory M)
	{
		int lengthBytes = (int)(M.Read<uint>(address + 16) * 2);
		uint num = M.Read<uint>(address + 24);
		if (8 <= num)
		{
			long addr = M.Read<long>(address);
			return M.ReadStringU(addr, lengthBytes);
		}
		return M.ReadStringU(address, lengthBytes);
	}
}
