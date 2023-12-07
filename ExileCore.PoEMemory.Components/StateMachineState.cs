namespace ExileCore.PoEMemory.Components;

public class StateMachineState
{
	public string Name { get; }

	public long Value { get; }

	public StateMachineState(string name, long value)
	{
		Name = name;
		Value = value;
	}

	public override string ToString()
	{
		return $"{Name}: {Value}";
	}
}
