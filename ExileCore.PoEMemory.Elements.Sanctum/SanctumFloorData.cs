using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.FilesInMemory.Sanctum;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Elements.Sanctum;

public class SanctumFloorData : RemoteMemoryObject
{
	public NativePtrArray RoomDataArray => base.M.Read<NativePtrArray>(base.Address + 24);

	public List<SanctumRoomData> RoomData => base.M.ReadStructsArray<SanctumRoomData>(RoomDataArray.First, RoomDataArray.Last, 112, null);

	public byte[][][] RoomLayout => (from x in base.M.ReadStdVectorStride<NativePtrArray>(base.M.Read<NativePtrArray>(base.Address), 32)
		select (from y in base.M.ReadStdVectorStride<NativePtrArray>(x, 56)
			select base.M.ReadStdVector<byte>(y)).ToArray()).ToArray();

	public List<SanctumDeferredReward> Rewards => (from x in base.M.ReadStdVectorStride<long>(base.M.Read<StdVector>(base.Address + 104), 16).Select(base.TheGame.Files.SanctumDeferredRewards.GetByAddress)
		where x != null
		select x).ToList();

	public List<byte> RoomChoices => base.M.ReadBytes(base.Address + 56, 8).TakeWhile((byte x) => x != byte.MaxValue).ToList();

	public short CurrentResolve => base.M.Read<short>(base.Address + 80);

	public short MaxResolve => base.M.Read<short>(base.Address + 82);

	public short Inspiration => base.M.Read<short>(base.Address + 84);

	public int Gold => base.M.Read<int>(base.Address + 72);
}
