using ExileCore.PoEMemory.FilesInMemory.Metamorph;

namespace ExileCore.PoEMemory.MemoryObjects.Metamorph;

public class MetamorphBodyPartElement : Element
{
	public MetamorphMetaSkill MetaSkill => ReadObjectAt<MetamorphMetaSkill>(1488);
}
