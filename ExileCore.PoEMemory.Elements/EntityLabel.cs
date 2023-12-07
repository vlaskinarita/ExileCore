using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements;

public class EntityLabel : Element
{
	public new string Text => NativeStringReader.ReadString(base.Address + 744, base.M);

	public string Text2 => NativeStringReader.ReadString(base.Address + 744, base.M);
}
