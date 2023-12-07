using System;
using System.Collections.Generic;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory;

public abstract class FileInMemory
{
	private readonly Func<long> fAddress;

	public IMemory M { get; }

	public long Address { get; }

	protected long FirstRecord => M.Read<long>(fAddress() + 64, new int[1]);

	protected long LastRecord => M.Read<long>(fAddress() + 64, new int[1] { 8 });

	protected virtual int NumberOfRecords => M.Read<int>(fAddress() + 64, new int[1] { 64 });

	protected virtual long RecordLength
	{
		get
		{
			int numberOfRecords = NumberOfRecords;
			if (numberOfRecords == 0)
			{
				return 0L;
			}
			return (LastRecord - FirstRecord) / numberOfRecords;
		}
	}

	protected FileInMemory(IMemory m, Func<long> address)
	{
		M = m;
		Address = address();
		fAddress = address;
	}

	protected IEnumerable<long> RecordAddresses()
	{
		if (fAddress() == 0L)
		{
			yield return 0L;
			yield break;
		}
		int cnt = NumberOfRecords;
		if (cnt == 0)
		{
			yield return 0L;
			yield break;
		}
		long firstRec = FirstRecord;
		long recLen = RecordLength;
		for (int i = 0; i < cnt; i++)
		{
			yield return firstRec + i * recLen;
		}
	}
}
