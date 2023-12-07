namespace ExileCore.PoEMemory.MemoryObjects.Heist;

public class HeistChestRewardTypeRecord : RemoteMemoryObject
{
	public string Id => base.M.ReadStringU(base.M.Read<long>(base.Address));

	public string Art => base.M.ReadStringU(base.M.Read<long>(base.Address + 8));

	public string Name => base.M.ReadStringU(base.M.Read<long>(base.Address + 16));

	public int MinimumDropLevel => base.M.Read<int>(base.Address + 40);

	public int MaximumDropLevel => base.M.Read<int>(base.Address + 44);

	public int Weight => base.M.Read<int>(base.Address + 48);

	public string RoomName => base.M.ReadStringU(base.M.Read<long>(base.Address + 52));

	public int RequiredJobLevel => base.M.Read<int>(base.Address + 60);

	public HeistJobRecord RequiredJob => base.TheGame.Files.HeistJobs.GetByAddress(base.M.Read<long>(base.Address + 68, new int[1] { 8 }));

	public override string ToString()
	{
		return Name;
	}
}
