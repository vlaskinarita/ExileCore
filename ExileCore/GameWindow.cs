using System;
using System.Diagnostics;
using System.Drawing;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using GameOffsets.Native;
using SharpDX;

namespace ExileCore;

public class GameWindow
{
	private readonly IntPtr handle;

	private readonly CachedValue<SharpDX.RectangleF> _getWindowRectangle;

	private System.Drawing.Rectangle _lastValid = System.Drawing.Rectangle.Empty;

	public Process Process { get; }

	public SharpDX.RectangleF GetWindowRectangleTimeCache => _getWindowRectangle.Value;

	public GameWindow(Process process)
	{
		Process = process;
		handle = process.MainWindowHandle;
		_getWindowRectangle = new TimeCache<SharpDX.RectangleF>(GetWindowRectangleReal, 200L);
	}

	public SharpDX.RectangleF GetWindowRectangle()
	{
		return _getWindowRectangle.Value;
	}

	public SharpDX.RectangleF GetWindowRectangleReal()
	{
		System.Drawing.Rectangle lastValid = WinApi.GetClientRectangle(handle);
		if (lastValid.Width < 0 && lastValid.Height < 0)
		{
			lastValid = _lastValid;
		}
		else
		{
			_lastValid = lastValid;
		}
		return new SharpDX.RectangleF(lastValid.X, lastValid.Y, lastValid.Width, lastValid.Height);
	}

	public bool IsForeground()
	{
		return WinApi.IsForegroundWindow(handle);
	}

	[Obsolete]
	public Vector2 ScreenToClient(int x, int y)
	{
		SharpDX.Point lpPoint = new SharpDX.Point(x, y);
		WinApi.ScreenToClient(handle, ref lpPoint);
		return lpPoint;
	}

	public Vector2i ScreenToClient(Vector2i screenCoords)
	{
		return WinApi.ScreenToClient(handle, screenCoords);
	}

	public Vector2i ScreenToClient2(int x, int y)
	{
		return WinApi.ScreenToClient(handle, new Vector2i(x, y));
	}
}
