using ExileCore.Shared.Cache;

namespace ExileCore.PoEMemory.FilesInMemory;

public class UniqueItemDescription : RemoteMemoryObject
{
	private record struct DataStruct(long NamePtr, long _, long VisualPtr);

	private ItemVisualIdentity _itemVisualIdentity;

	private WordEntry _uniqueName;

	private CachedValue<DataStruct> _data;

	public ItemVisualIdentity ItemVisualIdentity => _itemVisualIdentity ?? (_itemVisualIdentity = base.TheGame.Files.ItemVisualIdentities.GetByAddress(_data.Value.VisualPtr));

	public WordEntry UniqueName => _uniqueName ?? (_uniqueName = base.TheGame.Files.Words.GetByAddress(_data.Value.NamePtr));

	public UniqueItemDescription()
	{
		_data = new StaticValueCache<DataStruct>(() => base.M.Read<DataStruct>(base.Address));
	}

	public override string ToString()
	{
		return UniqueName?.Text ?? base.ToString();
	}
}
