using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;
using SharpDX;

namespace ExileCore.PoEMemory.MemoryObjects;

public class ServerStashTab : RemoteMemoryObject
{
	private static readonly int ColorOffset = Extensions.GetOffset((ServerStashTabOffsets x) => x.Color);

	private readonly CachedValue<ServerStashTabOffsets> _cachedValue;

	public ServerStashTabOffsets ServerStashTabOffsets => _cachedValue.Value;

	public string NameOld => NativeStringReader.ReadString(base.Address + 8, base.M) + (RemoveOnly ? " (Remove-only)" : string.Empty);

	public string Name => ServerStashTabOffsets.Name.ToString(base.M);

	public uint Color => ServerStashTabOffsets.Color;

	public Color Color2 => new Color(base.M.Read<byte>(base.Address + ColorOffset), base.M.Read<byte>(base.Address + ColorOffset + 1), base.M.Read<byte>(base.Address + ColorOffset + 2));

	public InventoryTabPermissions MemberFlags => (InventoryTabPermissions)ServerStashTabOffsets.MemberFlags;

	public InventoryTabPermissions OfficerFlags => (InventoryTabPermissions)ServerStashTabOffsets.OfficerFlags;

	public InventoryTabType TabType => (InventoryTabType)ServerStashTabOffsets.TabType;

	public ushort VisibleIndex => ServerStashTabOffsets.DisplayIndex;

	public InventoryTabFlags Flags => (InventoryTabFlags)ServerStashTabOffsets.Flags;

	public bool RemoveOnly => (Flags & InventoryTabFlags.RemoveOnly) == InventoryTabFlags.RemoveOnly;

	public bool IsHidden => (Flags & InventoryTabFlags.Hidden) == InventoryTabFlags.Hidden;

	public ServerStashTab()
	{
		_cachedValue = new FrameCache<ServerStashTabOffsets>(() => base.M.Read<ServerStashTabOffsets>(base.Address));
	}

	public override string ToString()
	{
		return $"{Name}, DisplayIndex: {VisibleIndex}, {TabType}";
	}
}
