using System;
using System.Diagnostics.CodeAnalysis;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class Buff : RemoteMemoryObject
{
	private readonly CachedValue<BuffOffsets> _cachedValue;

	private BuffDefinition _buffDefinition;

	public BuffOffsets BuffOffsets => _cachedValue.Value;

	public string Name => BuffDefinition?.Id ?? string.Empty;

	[Obsolete("Use BuffCharges instead")]
	public byte Charges => (byte)BuffOffsets.Charges;

	public ushort BuffCharges => BuffOffsets.Charges;

	public ushort BuffStacks => base.M.Read<ushort>(base.M.Read<long>(base.Address + 248));

	public string DisplayName => BuffDefinition?.Name ?? string.Empty;

	public string Description => BuffDefinition?.Description ?? string.Empty;

	public bool IsInvisible => BuffDefinition?.IsInvisible ?? false;

	public bool IsRemovable => BuffDefinition?.IsRemovable ?? false;

	public BuffDefinition BuffDefinition
	{
		[return: MaybeNull]
		get
		{
			return _buffDefinition ?? (_buffDefinition = base.TheGame.Files.BuffDefinitions.GetByAddress(BuffOffsets.BuffDatPtr));
		}
	}

	public float MaxTime => BuffOffsets.MaxTime;

	public float Timer => BuffOffsets.Timer;

	public Buff()
	{
		_cachedValue = new FramesCache<BuffOffsets>(() => base.M.Read<BuffOffsets>(base.Address), 3u);
	}

	public override string ToString()
	{
		return $"{DisplayName}({Name}) - Charges: {BuffCharges} MaxTime: {MaxTime}, BuffStacks: {BuffStacks}";
	}
}
