using System.Linq;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.MemoryObjects;

public class EnvironmentDataEnvironment : RemoteMemoryObject
{
	private const int EnvironmentNameSize = 32;

	public long EnvironmentDataAddress { get; internal set; }

	public string Name => base.M.Read<NativeUtf16Text>(base.Address).ToString(base.M);

	public TypedEnvironmentData<Type1EnvironmentSettingsOffsets> Type1Settings => new TypedEnvironmentData<Type1EnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 32, GetTypeNDefaultDataOffset(1), 94);

	public TypedEnvironmentData<Type2EnvironmentSettingsOffsets> Type2Settings => new TypedEnvironmentData<Type2EnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 784, GetTypeNDefaultDataOffset(2), 1);

	public TypedEnvironmentData<Type3EnvironmentSettingsOffsets> Type3Settings => new TypedEnvironmentData<Type3EnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 792, GetTypeNDefaultDataOffset(3), 8);

	public TypedEnvironmentData<Type4EnvironmentSettingsOffsets> Type4Settings => new TypedEnvironmentData<Type4EnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 920, GetTypeNDefaultDataOffset(4), 10);

	public TypedEnvironmentData<Type5EnvironmentSettingsOffsets> Type5Settings => new TypedEnvironmentData<Type5EnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 940, GetTypeNDefaultDataOffset(5), 2);

	public TypedEnvironmentData<Type6EnvironmentSettingsOffsets> Type6Settings => new TypedEnvironmentData<Type6EnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 960, GetTypeNDefaultDataOffset(6), 13);

	public TypedEnvironmentData<Type7PlusEnvironmentSettingsOffsets> Type7Settings => new TypedEnvironmentData<Type7PlusEnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 2000, GetTypeNDefaultDataOffset(7), 1);

	public TypedEnvironmentData<Type7PlusEnvironmentSettingsOffsets> Type8Settings => new TypedEnvironmentData<Type7PlusEnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 2032, GetTypeNDefaultDataOffset(8), 1);

	public TypedEnvironmentData<Type7PlusEnvironmentSettingsOffsets> Type9Settings => new TypedEnvironmentData<Type7PlusEnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 2064, GetTypeNDefaultDataOffset(9), 1);

	public TypedEnvironmentData<Type7PlusEnvironmentSettingsOffsets> Type10Settings => new TypedEnvironmentData<Type7PlusEnvironmentSettingsOffsets>(base.M, EnvironmentDataAddress, base.Address, 2096, GetTypeNDefaultDataOffset(10), 1);

	private int GetTypeNDefaultDataOffset(int n)
	{
		return new int[10] { 94, 1, 8, 10, 2, 13, 1, 1, 1, 1 }.Take(n - 1).Sum() * 8 + 24;
	}

	public override string ToString()
	{
		return Name ?? "";
	}
}
