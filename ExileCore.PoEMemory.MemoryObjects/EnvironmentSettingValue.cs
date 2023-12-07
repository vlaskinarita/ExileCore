namespace ExileCore.PoEMemory.MemoryObjects;

public class EnvironmentSettingValue<T> : StructuredRemoteMemoryObject<T> where T : unmanaged
{
	public DefaultEnvironmentSetting Default { get; internal set; }

	public override string ToString()
	{
		return $"{base.Structure} :{Default}";
	}
}
