using System;
using System.Runtime.CompilerServices;

namespace ExileCore.Shared.Helpers;

public static class IntPtrExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IntPtr Add(this IntPtr left, IntPtr right)
	{
		return new IntPtr((long)(nint)left + (long)(nint)right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IntPtr Divide(this IntPtr left, IntPtr right)
	{
		return new IntPtr((long)(nint)left / (long)(nint)right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong GetValue(this IntPtr ptr)
	{
		return (ulong)(nint)ptr;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAligned(this IntPtr ptr)
	{
		ulong value = ptr.GetValue();
		if (value != 1)
		{
			return value % 2uL == 0;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNotZero(this IntPtr ptr)
	{
		return ptr != IntPtr.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValid(this IntPtr ptr)
	{
		ulong num = (ulong)(nint)ptr;
		if (IntPtr.Size == 4)
		{
			if (num > 65536)
			{
				return num < 4293918720u;
			}
			return false;
		}
		if (num > 65536)
		{
			return num < 4222124650659840L;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsZero(this IntPtr ptr)
	{
		return ptr == IntPtr.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IntPtr Multiply(this IntPtr left, IntPtr right)
	{
		return new IntPtr((long)(nint)left * (long)(nint)right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IntPtr Subtract(this IntPtr left, IntPtr right)
	{
		return new IntPtr((long)(nint)left - (long)(nint)right);
	}
}
