using System;
using SharpDX;

namespace ExileCore.RenderQ;

public class SpritesRender
{
	private readonly DX11 _dx11;

	private readonly ImGuiRender _imGuiRender;

	public SpritesRender(DX11 dx11, ImGuiRender imGuiRender)
	{
		_dx11 = dx11;
		_imGuiRender = imGuiRender;
	}

	[Obsolete]
	public bool LoadImage(string fileName)
	{
		return _dx11.InitTexture(fileName);
	}

	[Obsolete]
	public void DrawImage(string fileName, RectangleF rect, RectangleF uv, Color color)
	{
		try
		{
			_imGuiRender.DrawImage(fileName, rect, uv, color);
		}
		catch (Exception ex)
		{
			DebugWindow.LogError(ex.ToString());
		}
	}
}
