using ExileCore.PoEMemory.Models;

namespace ExileCore.PoEMemory.FilesInMemory.Metamorph;

public class MetamorphMetaSkillType : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address), 255);

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 8), 255);

	public string Description => base.M.ReadStringU(base.M.Read<long>(base.Address + 16), 255);

	public BaseItemType BaseItemType => base.TheGame.Files.BaseItemTypes.GetFromAddress(base.M.Read<long>(base.Address + 56));

	public string BodyPart => base.M.ReadStringU(base.M.Read<long>(base.Address + 64), 255);

	public override string ToString()
	{
		return $"{BodyPart}, {Id}, {Name}, {BaseItemType?.BaseName}, {Description}";
	}
}
