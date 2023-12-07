using System;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory;

public class StatDescriptionWrapper<T> : UniversalFileWrapper<T> where T : RemoteMemoryObject, new()
{
	protected override long RecordLength => 8L;

	protected override int NumberOfRecords => (int)((base.LastRecord - base.FirstRecord) / RecordLength);

	public StatDescriptionWrapper(IMemory mem, Func<long> address)
		: base(mem, address)
	{
	}
}
