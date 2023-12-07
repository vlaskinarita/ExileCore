namespace ExileCore.PoEMemory.MemoryObjects;

public class SkillCooldown : RemoteMemoryObject
{
	public float Remaining => base.M.Read<float>(base.Address);

	public float TotalCooldown => base.M.Read<float>(base.Address + 8);
}
