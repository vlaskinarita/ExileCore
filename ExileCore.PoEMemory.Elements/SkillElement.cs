using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements;

public class SkillElement : Element
{
	private const int SkillPtrOffset = 632;

	public bool isValid => unknown1 != 0;

	public bool IsAssignedKeyOrIsActive => base.M.Read<int>(unknown1 + 8) > 3;

	public string SkillIconPath => base.M.ReadStringU(base.M.Read<long>(unknown1 + 16), 100).TrimEnd('0');

	public int totalUses => base.M.Read<int>(unknown3 + 80);

	public bool isUsing => base.M.Read<byte>(unknown3 + 8) > 2;

	private long unknown1 => base.M.Read<long>(base.Address + 580);

	private long unknown3 => base.M.Read<long>(base.Address + 812);

	public ActorSkill Skill => ReadObjectAt<ActorSkill>(632).SetActor(base.TheGame.IngameState.Data.LocalPlayer.GetComponent<Actor>());
}
