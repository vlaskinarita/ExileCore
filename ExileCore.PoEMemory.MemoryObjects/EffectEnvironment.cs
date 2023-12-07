using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class EffectEnvironment : StructuredRemoteMemoryObject<EnvironmentOffsets>
{
	public ushort Key => base.Structure.Key;

	public ushort Value0 => base.Structure.Value0;

	public float Value1 => base.Structure.Value1;
}
