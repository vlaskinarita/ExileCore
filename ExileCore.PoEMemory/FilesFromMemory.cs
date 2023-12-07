using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Interfaces;
using GameOffsets;
using MoreLinq;

namespace ExileCore.PoEMemory;

public class FilesFromMemory
{
	private readonly IMemory mem;

	public FilesFromMemory(IMemory memory)
	{
		mem = memory;
	}

	public Dictionary<string, FileInformation> GetAllFiles()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		Dictionary<string, FileInformation> allFilesSync = GetAllFilesSync();
		DebugWindow.LogMsg($"GetAllFiles loaded in {stopwatch.ElapsedMilliseconds}ms", 5f);
		return allFilesSync;
	}

	public Dictionary<string, FileInformation> GetAllFilesSync()
	{
		ConcurrentDictionary<string, FileInformation> files = new ConcurrentDictionary<string, FileInformation>();
		long addr = mem.AddressOfProcess + mem.BaseOffsets[OffsetsName.FileRoot];
		byte[] arrBytes = mem.ReadBytes(addr, 640);
		Parallel.For(0, 16, delegate(int i)
		{
			long addr2 = BitConverter.ToInt64(arrBytes, i * 40 + 8);
			byte[] array = mem.ReadBytes(addr2, 102400);
			for (int j = 0; j < 512; j++)
			{
				int num = j * 200;
				for (int k = 0; k < 8; k++)
				{
					if (array[num + k] != byte.MaxValue)
					{
						int num2 = 8 + k * 24 + 16;
						long num3 = BitConverter.ToInt64(array, num + num2);
						FileInfo fileInfo = mem.Read<FileInfoPadded>(num3).FileInfo;
						string key = mem.ReadStringU(fileInfo.Name);
						files.TryAdd(key, new FileInformation(num3, fileInfo.AreaChangeCount));
					}
				}
			}
		});
		return files.ToDictionary();
	}
}
