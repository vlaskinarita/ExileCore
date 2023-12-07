using ExileCore.Shared.Helpers;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Elements;

public class DelveCell : Element
{
	private DelveCellInfoStrings info;

	private NativeStringU mods => base.M.Read<NativeStringU>(base.Address + 1176);

	public string Mods => mods.ToString(base.M);

	private NativeStringU mines => base.M.Read<NativeStringU>(base.M.Read<long>(base.Address + 336) + 56);

	public string MinesText => mines.ToString(base.M);

	public DelveCellInfoStrings Info
	{
		get
		{
			DelveCellInfoStrings obj = info ?? ReadObjectAt<DelveCellInfoStrings>(1600);
			DelveCellInfoStrings result = obj;
			info = obj;
			return result;
		}
	}

	public string Type => base.M.ReadStringU(base.M.Read<long>(base.Address + 1616, new int[1]));

	public string TypeHuman => base.M.ReadStringU(base.M.Read<long>(base.Address + 1616, new int[1] { 8 }));

	public override string Text => Info.TestString + " [" + Info.TestString5 + "]";
}
