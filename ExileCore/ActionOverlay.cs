using System;
using System.Threading.Tasks;
using ClickableTransparentOverlay;

namespace ExileCore;

public class ActionOverlay : Overlay
{
	public Action RenderAction { get; set; }

	public Action PostFrameAction { get; set; }

	public Func<Task> PostInitializedAction { get; set; }

	public ActionOverlay(string windowTitle)
		: base(windowTitle)
	{
	}

	protected override void Render()
	{
		RenderAction?.Invoke();
	}

	protected override async Task PostInitialized()
	{
		if (PostInitializedAction != null)
		{
			await PostInitializedAction();
		}
	}

	protected override void PostFrame()
	{
		PostFrameAction?.Invoke();
	}
}
