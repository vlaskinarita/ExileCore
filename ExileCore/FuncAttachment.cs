using System;

namespace ExileCore;

internal class FuncAttachment : IDisposable
{
	private readonly Action<Action> _detachAction;

	private Action _attachedAction;

	public FuncAttachment(Action action, Action<Action> detachAction = null)
	{
		_detachAction = detachAction;
		_attachedAction = action;
	}

	private void Act()
	{
		if (_attachedAction != null)
		{
			_attachedAction();
		}
		else
		{
			Dispose();
		}
	}

	public void Dispose()
	{
		_attachedAction = null;
		_detachAction?.Invoke(Act);
	}

	public static FuncAttachment Attach(ref Action baseAction, Action attachedAction, Action<Action> detachAction = null)
	{
		FuncAttachment funcAttachment = new FuncAttachment(attachedAction, detachAction);
		baseAction = (Action)Delegate.Combine(baseAction, new Action(funcAttachment.Act));
		return funcAttachment;
	}
}
