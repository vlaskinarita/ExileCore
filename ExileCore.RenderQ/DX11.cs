using System;
using System.IO;
using System.Linq;
using System.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ExileCore.RenderQ;

public class DX11 : IDisposable
{
	private readonly ActionOverlay _overlay;

	private readonly ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();

	public ImGuiRender ImGuiRender { get; }

	[Obsolete]
	public SpritesRender SpritesRender { get; }

	public DX11(ActionOverlay overlay, CoreSettings coreSettings)
	{
		_overlay = overlay;
		ImGuiRender = new ImGuiRender(this, overlay, coreSettings);
		SpritesRender = new SpritesRender(this, ImGuiRender);
	}

	public void Dispose()
	{
		_sync.Dispose();
	}

	public void DisposeTexture(string name)
	{
		_sync.EnterWriteLock();
		try
		{
			if (!_overlay.RemoveImage(name))
			{
				DebugWindow.LogError($"({"DisposeTexture"}) Texture {name} not found.", 10f);
			}
		}
		finally
		{
			_sync.ExitWriteLock();
		}
	}

	public void AddOrUpdateTexture(string name, Image<Rgba32> image)
	{
		_sync.EnterWriteLock();
		try
		{
			_overlay.RemoveImage(name);
			_overlay.AddOrGetImagePointer(name, image, srgb: false, out var _);
		}
		finally
		{
			_sync.ExitWriteLock();
		}
	}

	public IntPtr GetTexture(string name)
	{
		_sync.EnterReadLock();
		try
		{
			return _overlay.GetImagePointer(name);
		}
		finally
		{
			_sync.ExitReadLock();
		}
	}

	public bool HasTexture(string name)
	{
		_sync.EnterReadLock();
		try
		{
			return _overlay.HasImagePointer(name);
		}
		finally
		{
			_sync.ExitReadLock();
		}
	}

	public bool InitTexture(string name)
	{
		if (!File.Exists(name))
		{
			DebugWindow.LogError(name + " not found.");
			return false;
		}
		_sync.EnterWriteLock();
		try
		{
			_overlay.AddOrGetImagePointer(name, name.Split('/', '\\').Last(), srgb: false, out var _, out var _, out var _);
		}
		finally
		{
			_sync.ExitWriteLock();
		}
		return true;
	}

	public bool InitTexture(string name, string path)
	{
		if (!File.Exists(path))
		{
			DebugWindow.LogError(path + " not found.");
			return false;
		}
		_sync.EnterWriteLock();
		try
		{
			_overlay.AddOrGetImagePointer(path, name, srgb: false, out var _, out var _, out var _);
		}
		finally
		{
			_sync.ExitWriteLock();
		}
		return true;
	}
}
