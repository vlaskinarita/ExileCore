using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Forms;
using ExileCore.Shared;
using ExileCore.Shared.Helpers;
using MoreLinq.Extensions;
using SharpDX;
using Vanara.PInvoke;

namespace ExileCore;

public class Input
{
	private const int ACTION_DELAY = 1;

	private const int KEY_PRESS_DELAY = 10;

	public const int MOUSEEVENTF_MOVE = 1;

	public const int MOUSEEVENTF_LEFTDOWN = 2;

	public const int MOUSEEVENTF_LEFTUP = 4;

	public const int MOUSEEVENTF_MIDDOWN = 32;

	public const int MOUSEEVENTF_MIDUP = 64;

	public const int MOUSEEVENTF_RIGHTDOWN = 8;

	public const int MOUSEEVENTF_RIGHTUP = 16;

	public const int MOUSE_EVENT_WHEEL = 2048;

	private static readonly Dictionary<Keys, bool> Keys;

	private static readonly HashSet<Keys> RegisteredKeys;

	private static readonly object locker;

	private static readonly WaitTime cursorPositionSmooth;

	private static readonly WaitTime keyPress;

	private static readonly Dictionary<Keys, bool> KeysPressed;

	private static readonly Stopwatch sw;

	[Obsolete]
	public static SharpDX.Vector2 ForceMousePosition => WinApi.GetCursorPositionPoint();

	public static System.Numerics.Vector2 ForceMousePositionNum => WinApi.GetCursorPositionPoint().ToVector2();

	[Obsolete]
	public static SharpDX.Vector2 MousePosition => MousePositionNum.ToSharpDx();

	public static System.Numerics.Vector2 MousePositionNum { get; private set; }

	public static event EventHandler<Keys> ReleaseKey;

	static Input()
	{
		Keys = new Dictionary<Keys, bool>();
		RegisteredKeys = new HashSet<Keys>();
		locker = new object();
		cursorPositionSmooth = new WaitTime(1);
		keyPress = new WaitTime(1);
		KeysPressed = new Dictionary<Keys, bool>();
		sw = Stopwatch.StartNew();
		Keys[] values = Enum.GetValues<Keys>();
		foreach (Keys key in values)
		{
			KeysPressed[key] = false;
		}
	}

	public static bool IsKeyDown(int nVirtKey)
	{
		return IsKeyDown((Keys)nVirtKey);
	}

	public static bool IsKeyDown(Keys nVirtKey)
	{
		if (nVirtKey == System.Windows.Forms.Keys.None)
		{
			return false;
		}
		if (!Keys.ContainsKey(nVirtKey))
		{
			RegisterKey(nVirtKey);
		}
		return Keys[nVirtKey];
	}

	public static bool GetKeyState(Keys key)
	{
		if (key == System.Windows.Forms.Keys.None)
		{
			return false;
		}
		return WinApi.GetKeyState(key) < 0;
	}

	public static void RegisterKey(Keys key)
	{
		if (key == System.Windows.Forms.Keys.None || Keys.TryGetValue(key, out var _))
		{
			return;
		}
		lock (locker)
		{
			Keys[key] = false;
			RegisteredKeys.Add(key);
		}
	}

	public static void Update(IntPtr? windowPtr)
	{
		if (windowPtr.HasValue)
		{
			MousePositionNum = WinApi.GetCursorPosition(windowPtr.Value).ToVector2();
		}
		try
		{
			RegisteredKeys.ForEach(delegate(Keys key)
			{
				bool keyState = GetKeyState(key);
				if (!keyState && Keys[key])
				{
					Input.ReleaseKey?.Invoke(null, key);
				}
				Keys[key] = keyState;
			});
		}
		catch (Exception value)
		{
			DebugWindow.LogMsg($"{"Input"} {value}");
		}
	}

	[Obsolete]
	public static IEnumerator SetCursorPositionSmooth(SharpDX.Vector2 vec)
	{
		float step = Math.Max(vec.Distance(ForceMousePosition) / 100f, 4f);
		if (step > 6f)
		{
			for (int i = 0; (float)i < step; i++)
			{
				SetCursorPos(SharpDX.Vector2.SmoothStep(ForceMousePosition, vec, (float)i / step));
				MouseMove();
				yield return cursorPositionSmooth;
			}
		}
		else
		{
			SetCursorPos(vec);
		}
	}

	[Obsolete]
	public static IEnumerator SetCursorPositionAndClick(SharpDX.Vector2 vec, MouseButtons button = MouseButtons.Left, int delay = 3)
	{
		SetCursorPos(vec);
		yield return new WaitTime(delay);
		Click(button);
		yield return new WaitTime(delay);
	}

	public static void VerticalScroll(bool forward, int clicks)
	{
		if (forward)
		{
			int dwData = clicks * 120;
			User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_WHEEL, 0, 0, dwData, (IntPtr)0);
		}
		else
		{
			int dwData2 = -(clicks * 120);
			User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_WHEEL, 0, 0, dwData2, (IntPtr)0);
		}
	}

	[Obsolete]
	public static void SetCursorPos(SharpDX.Vector2 vec)
	{
		User32.SetCursorPos((int)vec.X, (int)vec.Y);
		MouseMove();
	}

	public static void SetCursorPos(System.Numerics.Vector2 vec)
	{
		User32.SetCursorPos((int)vec.X, (int)vec.Y);
		MouseMove();
	}

	public static void Click(MouseButtons buttons)
	{
		switch (buttons)
		{
		case MouseButtons.Left:
			MouseMove();
			User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN | User32.MOUSEEVENTF.MOUSEEVENTF_LEFTUP, 0, 0, 0, (IntPtr)0);
			break;
		case MouseButtons.Right:
			MouseMove();
			User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_RIGHTDOWN | User32.MOUSEEVENTF.MOUSEEVENTF_RIGHTUP, 0, 0, 0, (IntPtr)0);
			break;
		}
	}

	public static void LeftDown()
	{
		User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, (IntPtr)0);
	}

	public static void LeftUp()
	{
		User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_LEFTUP, 0, 0, 0, (IntPtr)0);
	}

	public static void MouseMove()
	{
		User32.mouse_event(User32.MOUSEEVENTF.MOUSEEVENTF_MOVE, 0, 0, 0, (IntPtr)0);
	}

	public static IEnumerator KeyPress(Keys key)
	{
		if (key != 0)
		{
			KeyDown(key);
			yield return keyPress;
			KeyUp(key);
		}
	}

	public static IEnumerator KeyPress(Keys key, int ms = 5)
	{
		if (key != 0)
		{
			KeyDown(key);
			yield return new WaitTime(ms);
			KeyUp(key);
		}
	}

	public static void KeyDown(Keys key)
	{
		if (key != 0)
		{
			User32.keybd_event((byte)key, 0, User32.KEYEVENTF.KEYEVENTF_EXTENDEDKEY, (IntPtr)0);
		}
	}

	public static void KeyUp(Keys key)
	{
		if (key != 0)
		{
			User32.keybd_event((byte)key, 0, User32.KEYEVENTF.KEYEVENTF_EXTENDEDKEY | User32.KEYEVENTF.KEYEVENTF_KEYUP, (IntPtr)0);
		}
	}

	public static void KeyDown(Keys key, IntPtr handle)
	{
		if (key != 0)
		{
			WinApi.SendMessage(handle, 256, (int)key, 0);
		}
	}

	public static void KeyUp(Keys key, IntPtr handle)
	{
		if (key != 0)
		{
			WinApi.SendMessage(handle, 257, (int)key, 0);
		}
	}

	public static void KeyPressRelease(Keys key, IntPtr handle)
	{
		if (key == System.Windows.Forms.Keys.None)
		{
			return;
		}
		if (sw.ElapsedMilliseconds >= 10 && KeysPressed[key])
		{
			KeyUp(key, handle);
			lock (locker)
			{
				KeysPressed[key] = false;
			}
			sw.Restart();
		}
		else if (!KeysPressed[key])
		{
			KeyDown(key, handle);
			lock (locker)
			{
				KeysPressed[key] = true;
			}
			sw.Restart();
		}
	}

	public static void KeyPressRelease(Keys key)
	{
		if (key == System.Windows.Forms.Keys.None)
		{
			return;
		}
		if (sw.ElapsedMilliseconds >= 10 && KeysPressed[key])
		{
			KeyUp(key);
			lock (locker)
			{
				KeysPressed[key] = false;
			}
			sw.Restart();
		}
		else if (!KeysPressed[key])
		{
			KeyDown(key);
			lock (locker)
			{
				KeysPressed[key] = true;
			}
			sw.Restart();
		}
	}
}
