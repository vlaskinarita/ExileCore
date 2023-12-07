using System;
using System.Diagnostics;

namespace ExileCore;

public class Time
{
	private static Stopwatch Stopwatch { get; } = Stopwatch.StartNew();


	public static double TotalMilliseconds => Stopwatch.Elapsed.TotalMilliseconds;

	public static long ElapsedMilliseconds => Stopwatch.ElapsedMilliseconds;

	public static TimeSpan Elapsed => Stopwatch.Elapsed;
}
