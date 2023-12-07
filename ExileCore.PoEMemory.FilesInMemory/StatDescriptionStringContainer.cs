using System.Runtime.InteropServices;

namespace ExileCore.PoEMemory.FilesInMemory;

[StructLayout(LayoutKind.Explicit, Size = 168)]
public struct StatDescriptionStringContainer
{
	[FieldOffset(112)]
	public long StringPtr;
}
