using ExileCore.Shared.Cache;
using GameOffsets;
using ProcessMemoryUtilities.Managed;

namespace ExileCore.PoEMemory.MemoryObjects;

public class DiagnosticElement : RemoteMemoryObject
{
	private readonly CachedValue<DiagnosticElementOffsets> _cachedValue;

	private readonly CachedValue<DiagnosticElementArrayOffsets> _cachedValue2;

	private readonly FrameCache<float[]> Values;

	private DiagnosticElementOffsets DiagnosticElementStruct => _cachedValue.Value;

	private DiagnosticElementArrayOffsets DiagnosticElementArrayStruct => _cachedValue2.Value;

	public long DiagnosticArray => DiagnosticElementStruct.DiagnosticArray;

	public float[] DiagnosticArrayValues => Values.Value;

	public float CurrValue => DiagnosticElementArrayStruct.CurrValue;

	public int X => DiagnosticElementStruct.X;

	public int Y => DiagnosticElementStruct.Y;

	public int Width => DiagnosticElementStruct.Width;

	public int Height => DiagnosticElementStruct.Height;

	public DiagnosticElement()
	{
		_cachedValue = new FrameCache<DiagnosticElementOffsets>(() => base.M.Read<DiagnosticElementOffsets>(base.Address));
		_cachedValue2 = new FrameCache<DiagnosticElementArrayOffsets>(() => base.M.Read<DiagnosticElementArrayOffsets>(DiagnosticElementStruct.DiagnosticArray));
		Values = new FrameCache<float[]>(delegate
		{
			float[] array = new float[80];
			NativeWrapper.ReadProcessMemoryArray(base.M.OpenProcessHandle, (nint)DiagnosticElementStruct.DiagnosticArray, array, 0, 80);
			return array;
		});
	}
}
