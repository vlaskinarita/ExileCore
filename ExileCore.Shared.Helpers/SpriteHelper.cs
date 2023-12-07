using ExileCore.Shared.Enums;
using SharpDX;

namespace ExileCore.Shared.Helpers;

public static class SpriteHelper
{
	public static RectangleF GetUV(MyMapIconsIndex index)
	{
		return GetUV((int)index, Constants.MyMapIcons);
	}

	public static RectangleF GetUV(MapIconsIndex index)
	{
		return GetUV((int)index, Constants.MapIconsSize);
	}

	public static RectangleF GetUV(int index, Size2F size)
	{
		if (index % (int)size.Width == 0)
		{
			return new RectangleF((size.Width - 1f) / size.Width, (float)((int)((float)index / size.Width) - 1) / size.Height, 1f / size.Width, 1f / size.Height);
		}
		return new RectangleF(((float)index % size.Width - 1f) / size.Width, (float)(index / (int)size.Width) / size.Height, 1f / size.Width, 1f / size.Height);
	}

	public static RectangleF GetUV(Size2 index, Size2F size)
	{
		return new RectangleF((float)(index.Width - 1) / size.Width, (float)(index.Height - 1) / size.Height, 1f / size.Width, 1f / size.Height);
	}

	public static RectangleF GetUV(int x, int y, float width, float height)
	{
		return new RectangleF((float)(x - 1) / width, (float)(y - 1) / height, 1f / width, 1f / height);
	}
}
