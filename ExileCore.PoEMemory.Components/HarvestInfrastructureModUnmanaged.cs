using System.Runtime.InteropServices;

namespace ExileCore.PoEMemory.Components;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
internal struct HarvestInfrastructureModUnmanaged
{
	[FieldOffset(0)]
	public long DatPtrUnused;

	[FieldOffset(8)]
	public long DatEntryPtr;

	[FieldOffset(16)]
	public int ModLevel;

	[FieldOffset(20)]
	public int Unknown;
}
