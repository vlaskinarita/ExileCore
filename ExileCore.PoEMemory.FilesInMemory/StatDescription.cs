using System.Collections.Generic;
using System.Linq;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.FilesInMemory;

public class StatDescription : RemoteMemoryObject
{
	private List<string> _strings;

	public List<string> Strings => _strings ?? (_strings = (from x in base.M.ReadStdVector<StatDescriptionStringContainer>(base.M.Read<StdVector>(base.Address, new int[1] { 16 }))
		select base.M.ReadStringU(x.StringPtr)).ToList());
}
