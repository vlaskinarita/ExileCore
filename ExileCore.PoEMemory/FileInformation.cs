namespace ExileCore.PoEMemory;

public struct FileInformation
{
	public long Ptr { get; }

	public int ChangeCount { get; }

	public FileInformation(long ptr, int changeCount)
	{
		Ptr = ptr;
		ChangeCount = changeCount;
	}
}
