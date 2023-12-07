using System;
using System.Diagnostics;
using JM.LinqFaster;

namespace ExileCore.Shared;

public class DebugInformation
{
	public class MeasureHolder : IDisposable
	{
		private readonly DebugInformation _debugInformation;

		private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

		private bool _disposed;

		public TimeSpan Elapsed => _stopwatch.Elapsed;

		public MeasureHolder(DebugInformation debugInformation)
		{
			_debugInformation = debugInformation;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_stopwatch.Stop();
				_debugInformation.Tick = _stopwatch.Elapsed.TotalMilliseconds;
				_disposed = true;
			}
		}
	}

	public static readonly int SizeArray = 512;

	private readonly Stopwatch sw = Stopwatch.StartNew();

	private double tick;

	public string Name { get; }

	public string Description { get; }

	public bool Main { get; }

	public int IndexTickAverage { get; private set; }

	public int Index { get; private set; }

	public float Sum { get; private set; }

	public float Total { get; private set; }

	private float TotalIndex { get; set; }

	public float TotalMaxAverage { get; private set; }

	public float TotalAverage { get; private set; }

	public float Average { get; private set; }

	public bool AtLeastOneFullTick { get; private set; }

	public double Tick
	{
		get
		{
			return tick;
		}
		set
		{
			tick = value;
			if (Index >= SizeArray)
			{
				Index = 0;
				Sum = Ticks.SumF();
				TotalIndex += SizeArray;
				Total += Sum;
				Average = Sum / (float)SizeArray;
				TotalAverage = Total / TotalIndex;
				TotalMaxAverage = Math.Max(TotalMaxAverage, Average);
				if (IndexTickAverage >= SizeArray)
				{
					IndexTickAverage = 0;
				}
				if (IndexTickAverage == 0 && Average > 16f)
				{
					Average = 0f;
					TotalMaxAverage = 0f;
				}
				TicksAverage[IndexTickAverage] = Average;
				IndexTickAverage++;
				AtLeastOneFullTick = true;
			}
			Ticks[Index] = (float)value;
			Index++;
		}
	}

	public float[] Ticks { get; } = new float[SizeArray];


	public float[] TicksAverage { get; } = new float[SizeArray];


	public DebugInformation(string name, bool main = true)
	{
		Name = name;
		Main = main;
		for (int i = 0; i < SizeArray; i++)
		{
			Ticks[i] = 0f;
			TicksAverage[i] = 0f;
		}
		lock (Core.SyncLocker)
		{
			Core.DebugInformations.Add(this);
		}
	}

	public DebugInformation(string name, string description, bool main = true)
		: this(name, main)
	{
		Description = description;
	}

	public MeasureHolder Measure()
	{
		return new MeasureHolder(this);
	}

	public void CorrectAfterTick(float val)
	{
		Ticks[Index - 1] = val;
		tick += val;
	}

	public float TickAction(Action action, bool onlyValue = false)
	{
		double totalMilliseconds = sw.Elapsed.TotalMilliseconds;
		action();
		float num = (float)(sw.Elapsed.TotalMilliseconds - totalMilliseconds);
		if (!onlyValue)
		{
			Tick = num;
		}
		return num;
	}

	public void AddToCurrentTick(float value)
	{
		Ticks[Index] += value;
	}
}
