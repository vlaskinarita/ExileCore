using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Cache;
using SharpDX;

namespace ExileCore.PoEMemory.Elements;

public class DelveBigCell : Element
{
	private readonly CachedValue<IList<DelveCell>> _cachedValue;

	private RectangleF rect = RectangleF.Empty;

	private string text;

	private long? type;

	public IList<DelveCell> Cells => _cachedValue.Value;

	public long TypePtr
	{
		get
		{
			long? num = type;
			if (!num.HasValue)
			{
				long? num2 = (type = base.M.Read<long>(base.Address + 336));
				return num2.Value;
			}
			return num.GetValueOrDefault();
		}
	}

	public override string Text
	{
		get
		{
			string obj = text ?? base.M.ReadStringU(base.M.Read<long>(TypePtr));
			string result = obj;
			text = obj;
			return result;
		}
	}

	public DelveBigCell()
	{
		_cachedValue = new ConditionalCache<IList<DelveCell>>(() => base.Children.Select((Element x) => x.AsObject<DelveCell>()).ToList(), delegate
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
