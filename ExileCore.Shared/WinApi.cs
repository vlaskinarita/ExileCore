using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GameOffsets.Native;
using ProcessMemoryUtilities.Native;
using SharpDX;
using Vanara.PInvoke;

namespace ExileCore.Shared;

public static class WinApi
{
	public const int SW_HIDE = 0;

	public const int SW_SHOW = 5;

	public const int SW_SHOWNORMAL = 1;

	public const int SW_SHOWMAXIMIZED = 3;

	public const int SW_RESTORE = 9;

	private const int WS_EX_LAYERED = 524288;

	private const int WS_EX_TRANSPARENT = 32;

	private const int WS_EX_TOPMOST = 8;

	private const int WS_VISIBLE = 268435456;

	public static void EnableTransparent(IntPtr handle)
	{
		User32.SetWindowLong(handle, User32.WindowLongFlags.GWL_STYLE, 268435456);
		User32.SetWindowLong(handle, User32.WindowLongFlags.GWL_EXSTYLE, 524288);
		HWND hWnd = handle;
		DwmApi.MARGINS pMarInset = new DwmApi.MARGINS(-1);
		DwmApi.DwmExtendFrameIntoClientArea(hWnd, in pMarInset);
	}

	public static void SetTransparent(IntPtr handle)
	{
		User32.SetWindowLong(handle, User32.WindowLongFlags.GWL_STYLE, 268435456);
		User32.SetWindowLong(handle, User32.WindowLongFlags.GWL_EXSTYLE, 524328);
	}

	public static void SetNoTransparent(IntPtr handle)
	{
		User32.SetWindowLong(handle, User32.WindowLongFlags.GWL_STYLE, 268435456);
		User32.SetWindowLong(handle, User32.WindowLongFlags.GWL_EXSTYLE, 524296);
	}

	public static void EnableTransparentByColorRef(IntPtr handle, System.Drawing.Rectangle size, int color)
	{
		int dwNewLong = User32.GetWindowLong(handle, User32.WindowLongFlags.GWL_EXSTYLE) | 0x80000;
		User32.SetWindowLong(handle, User32.WindowLongFlags.GWL_EXSTYLE, dwNewLong);
		User32.SetLayeredWindowAttributes(handle, (uint)color, 100, User32.LayeredWindowAttributes.LWA_ALPHA | User32.LayeredWindowAttributes.LWA_COLORKEY);
		HWND hWnd = handle;
		DwmApi.MARGINS pMarInset = new DwmApi.MARGINS(size.Left, size.Right, size.Top, size.Bottom);
		DwmApi.DwmExtendFrameIntoClientArea(hWnd, in pMarInset);
	}

	public static IntPtr OpenProcess(Process process, ProcessAccessFlags flags)
	{
		return Vanara.PInvoke.Kernel32.OpenProcess(new ACCESS_MASK((uint)flags), bInheritHandle: false, (uint)process.Id).ReleaseOwnership();
	}

	public static bool IsForegroundWindow(IntPtr handle)
	{
		return User32.GetForegroundWindow().DangerousGetHandle() == handle;
	}

	public static bool SetForegroundWindow(IntPtr hWnd)
	{
		return User32.SetForegroundWindow(hWnd);
	}

	public static bool AllowSetForegroundWindow(uint dwProcessId)
	{
		return User32.AllowSetForegroundWindow(dwProcessId);
	}

	public static bool ShowWindow(IntPtr hWnd, int nCmdShow)
	{
		return User32.ShowWindow(hWnd, (ShowWindowCommand)nCmdShow);
	}

	public static IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags)
	{
		if (!User32.SetWindowPos(hWnd, new IntPtr(hWndInsertAfter), x, y, cx, cy, (User32.SetWindowPosFlags)wFlags))
		{
			return new IntPtr(0);
		}
		return new IntPtr(1);
	}

	public static System.Drawing.Rectangle GetClientRectangle(IntPtr handle)
	{
		User32.GetClientRect(handle, out var lpRect);
		POINT lpPoint = default(POINT);
		User32.ClientToScreen(handle, ref lpPoint);
		return new System.Drawing.Rectangle(lpPoint.X, lpPoint.Y, lpRect.Width, lpRect.Height);
	}

	public static int MakeCOLORREF(byte r, byte g, byte b)
	{
		return r | (g << 8) | (b << 16);
	}

	public static bool ScreenToClient(IntPtr hWnd, ref SharpDX.Point lpPoint)
	{
		POINT lpPoint2 = new POINT(lpPoint.X, lpPoint.Y);
		bool result = User32.ScreenToClient(hWnd, ref lpPoint2);
		lpPoint = new SharpDX.Point(lpPoint2.X, lpPoint2.Y);
		return result;
	}

	public static Vector2i ScreenToClient(IntPtr hWnd, Vector2i lpPoint)
	{
		POINT lpPoint2 = new POINT(lpPoint.X, lpPoint.Y);
		User32.ScreenToClient(hWnd, ref lpPoint2);
		return new Vector2i(lpPoint2.X, lpPoint2.Y);
	}

	public static short GetKeyState(Keys vKey)
	{
		return User32.GetKeyState((int)vKey);
	}

	public static short GetAsyncKeyState(Keys vKey)
	{
		return User32.GetAsyncKeyState((int)vKey);
	}

	public static bool GetCursorPos(out SharpDX.Point lpPoint)
	{
		POINT lpPoint2;
		bool cursorPos = User32.GetCursorPos(out lpPoint2);
		lpPoint = new SharpDX.Point(lpPoint2.X, lpPoint2.Y);
		return cursorPos;
	}

	public static bool CloseHandle(IntPtr hObject)
	{
		return Vanara.PInvoke.Kernel32.CloseHandle(hObject);
	}

	public static bool IsIconic(IntPtr hWnd)
	{
		return User32.IsIconic(hWnd);
	}

	public static SharpDX.Point GetCursorPosition(IntPtr hWnd)
	{
		GetCursorPos(out var lpPoint);
		ScreenToClient(hWnd, ref lpPoint);
		return lpPoint;
	}

	public static SharpDX.Point GetCursorPositionPoint()
	{
		GetCursorPos(out var lpPoint);
		return lpPoint;
	}

	public static bool SetCursorPos(int x, int y)
	{
		return User32.SetCursorPos(x, y);
	}

	public static uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo)
	{
		User32.keybd_event(bVk, bScan, (User32.KEYEVENTF)dwFlags, (IntPtr)dwExtraInfo);
		return 0u;
	}

	public static void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo)
	{
		User32.mouse_event((User32.MOUSEEVENTF)dwFlags, dx, dy, cButtons, (IntPtr)dwExtraInfo);
	}

	public static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam)
	{
		return User32.SendMessage(hWnd, msg, (IntPtr)wParam, (IntPtr)lParam);
	}
}
