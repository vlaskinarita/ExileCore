using System.Collections.Generic;
using System.Linq;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class EnvironmentData : StructuredRemoteMemoryObject<EnvironmentDataOffsets>
{
	public List<DefaultEnvironmentSetting> DefaultSettingMap => base.M.ReadStdVector<long>(base.Structure.DefaultSettingsList).Select(RemoteMemoryObject.GetObjectStatic<DefaultEnvironmentSetting>).ToList();

	public List<EnvironmentDataEnvironment> Environments => base.M.ReadStdVector<long>(base.Structure.ActiveEnvironmentList).Select(base.GetObject<EnvironmentDataEnvironment>).Select(delegate(EnvironmentDataEnvironment x)
	{
		x.EnvironmentDataAddress = base.Address;
		return x;
	})
		.ToList();
}
