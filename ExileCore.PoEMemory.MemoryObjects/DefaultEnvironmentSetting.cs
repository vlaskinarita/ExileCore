using System.Numerics;
using ExileCore.Shared.Helpers;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class DefaultEnvironmentSetting : StructuredRemoteMemoryObject<DefaultEnvironmentSettingsOffsets>
{
	public string Category => base.Structure.Category.ToString(base.M);

	public string Name => base.Structure.Name.ToString(base.M);

	public int IndexInGroup => base.Structure.IndexInGroup;

	public Vector3 Value => base.Structure.Value;

	public override string ToString()
	{
		return $"{Category}:{Name} ({IndexInGroup}) = {Value}";
	}
}
