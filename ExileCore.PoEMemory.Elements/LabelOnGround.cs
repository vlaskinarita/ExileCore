using System;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements;

public class LabelOnGround : RemoteMemoryObject
{
	private readonly Lazy<string> debug;

	private readonly Lazy<long> labelInfo;

	public bool IsVisible => Label?.IsVisible ?? false;

	public bool IsVisibleLocal => Label?.IsVisibleLocal ?? false;

	public Entity ItemOnGround => ReadObjectAt<Entity>(24);

	public Element Label => ReadObjectAt<Element>(16);

	public bool CanPickUp => true;

	public TimeSpan TimeLeft => TimeSpan.Zero;

	public TimeSpan MaxTimeForPickUp => TimeSpan.Zero;

	public LabelOnGround()
	{
		labelInfo = new Lazy<long>(GetLabelInfo);
		debug = new Lazy<string>(() => (!ItemOnGround.HasComponent<WorldItem>()) ? ItemOnGround.Path : ItemOnGround.GetComponent<WorldItem>().ItemEntity?.GetComponent<Base>()?.Name);
	}

	private long GetLabelInfo()
	{
		if (Label == null)
		{
			return 0L;
		}
		if (Label.Address == 0L)
		{
			return 0L;
		}
		return base.M.Read<long>(Label.Address + 952);
	}

	public override string ToString()
	{
		return debug.Value;
	}
}
