using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Cache;
using SharpDX;

namespace ExileCore.PoEMemory.Elements;

public class DelveElement : Element
{
	private readonly CachedValue<IList<DelveBigCell>> _cachedValue;

	private RectangleF rect = RectangleF.Empty;

	public IList<DelveBigCell> Cells => _cachedValue.Value;

	public DelveElement()
	{
		_cachedValue = new ConditionalCache<IList<DelveBigCell>>(() => base.Children.Select((Element x) => x.AsObject<DelveBigCell>()).ToList(), delegate
		{
			if (GetClientRect() != rect)
			{
				rect = GetClientRect();
				return true;
			}
			return false;
		});
	}
}
