using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements;

public class HPbarElement : Element
{
	public Entity MonsterEntity => ReadObject<Entity>(base.Address + 2412);

	public new List<HPbarElement> Children => GetChildren<HPbarElement>().Cast<HPbarElement>().ToList();
}
