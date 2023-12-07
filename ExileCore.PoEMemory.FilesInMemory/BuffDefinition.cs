namespace ExileCore.PoEMemory.FilesInMemory;

public class BuffDefinition : RemoteMemoryObject
{
	private string _id;

	private string _description;

	private BuffVisual _buffVisual;

	private string _name;

	private bool? _isInvisible;

	private bool? _isRemovable;

	public string Id => _id ?? (_id = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public string Description => _description ?? (_description = base.M.ReadStringU(base.M.Read<long>(base.Address + 8)));

	public bool IsInvisible
	{
		get
		{
			bool valueOrDefault = _isInvisible.GetValueOrDefault();
			if (!_isInvisible.HasValue)
			{
				valueOrDefault = base.M.Read<bool>(base.Address + 16);
				_isInvisible = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public bool IsRemovable
	{
		get
		{
			bool valueOrDefault = _isRemovable.GetValueOrDefault();
			if (!_isRemovable.HasValue)
			{
				valueOrDefault = base.M.Read<bool>(base.Address + 17);
				_isRemovable = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public string Name => _name ?? (_name = base.M.ReadStringU(base.M.Read<long>(base.Address + 18)));

	public BuffVisual BuffVisual => _buffVisual ?? (_buffVisual = base.TheGame.Files.BuffVisuals.GetByAddress(base.M.Read<long>(base.Address + 85)));

	public override string ToString()
	{
		return $"{Id} {Name} {BuffVisual?.DdsFile} ({base.Address:X})";
	}
}
