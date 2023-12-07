using System.Collections.Generic;

namespace ExileCore.PoEMemory.Elements;

public class SkillBarElement : Element
{
	public long TotalSkills => base.ChildCount;

	public List<SkillElement> Skills => GetChildrenAs<SkillElement>();

	public new SkillElement this[int k] => base.Children[k].AsObject<SkillElement>();
}
