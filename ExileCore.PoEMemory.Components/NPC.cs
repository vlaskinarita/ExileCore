namespace ExileCore.PoEMemory.Components;

public class NPC : Component
{
	public bool HasIconOverhead => base.M.Read<long>(base.Address + 72) != 0;

	public bool IsIgnoreHidden => base.M.Read<byte>(base.Address + 32) == 1;

	public bool IsMinMapLabelVisible => base.M.Read<byte>(base.Address + 33) == 1;
}
