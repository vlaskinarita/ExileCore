using System.Collections.Generic;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class StateMachine : Component
{
	private readonly CachedValue<StateMachineComponentOffsets> _stateMachine;

	private readonly CachedValue<IList<StateMachineState>> _statesCache;

	public IList<StateMachineState> States => _statesCache.Value;

	public bool CanBeTarget => base.M.Read<byte>(base.Address + 160) == 1;

	public bool InTarget => base.M.Read<byte>(base.Address + 162) == 1;

	public StateMachine()
	{
		_stateMachine = new FrameCache<StateMachineComponentOffsets>(() => (base.Address != 0L) ? base.M.Read<StateMachineComponentOffsets>(base.Address) : default(StateMachineComponentOffsets));
		_statesCache = new FrameCache<IList<StateMachineState>>(CacheUtils.RememberLastValue<IList<StateMachineState>>(ReadStates));
	}

	public IList<StateMachineState> ReadStates()
	{
		return ReadStates(null);
	}

	private IList<StateMachineState> ReadStates(IList<StateMachineState> lastValue)
	{
		StateMachineComponentOffsets value = _stateMachine.Value;
		long num = value.StatesValues.ElementCount<long>();
		List<StateMachineState> list = new List<StateMachineState>();
		if (num <= 0)
		{
			if (lastValue == null || lastValue.Count <= 0 || !value.StatesValues.Equals(default(NativePtrArray)))
			{
				return list;
			}
			return lastValue;
		}
		if (num > 100)
		{
			Logger.Log.Error("Error reading states in StateMachine component");
			return list;
		}
		long[] array = base.M.ReadStdVector<long>(value.StatesValues);
		long num2 = base.M.Read<long>(value.StatesPtr + 16);
		for (int i = 0; i < num; i++)
		{
			long addr = num2 + i * 192;
			string name = base.M.Read<NativeUtf8Text>(addr).ToString(base.M);
			long value2 = array[i];
			list.Add(new StateMachineState(name, value2));
		}
		return list;
	}
}
