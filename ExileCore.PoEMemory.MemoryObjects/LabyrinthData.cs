using System.Collections.Generic;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects;

public class LabyrinthData : RemoteMemoryObject
{
	public IList<LabyrinthRoom> Rooms
	{
		get
		{
			long num = base.M.Read<long>(base.Address);
			long num2 = base.M.Read<long>(base.Address + 8);
			List<LabyrinthRoom> list = new List<LabyrinthRoom>();
			Dictionary<long, LabyrinthRoom> dictionary = new Dictionary<long, LabyrinthRoom>();
			for (long num3 = num; num3 < num2; num3 += 96)
			{
				DebugWindow.LogMsg($"Room Addr: {num3:X}", 0f, Color.White);
				if (num3 != 0L)
				{
					LabyrinthRoom @object = GetObject<LabyrinthRoom>(num3);
					@object.RoomCache = dictionary;
					list.Add(@object);
					dictionary.Add(num3, @object);
				}
			}
			return list;
		}
	}
}
