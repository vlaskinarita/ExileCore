using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExileCore.Shared.Helpers;
using MoreLinq.Extensions;

namespace ExileCore;

public class PagedMemoryBackend : IMemoryBackend, IDisposable
{
	private readonly ConcurrentDictionary<IntPtr, IMemoryOwner<byte>> _cachedPages = new ConcurrentDictionary<IntPtr, IMemoryOwner<byte>>();

	private readonly ConcurrentDictionary<IntPtr, bool> _pagesRequestedOnLastIteration = new ConcurrentDictionary<IntPtr, bool>();

	private CancellationTokenSource _nextFrameCts;

	private bool _disposed;

	private const int PageSize = 4096;

	private readonly IMemoryBackend _pageBackend;

	private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

	public PagedMemoryBackend(IMemoryBackend pageBackend)
	{
		_pageBackend = pageBackend;
	}

	private IntPtr GetPageAddress(IntPtr address)
	{
		return (nint)(address.GetValue() / 4096uL * 4096);
	}

	private IMemoryOwner<byte> GetRealPage(IntPtr address)
	{
		return GetRealPage(address, 1);
	}

	private IMemoryOwner<byte> GetRealPage(IntPtr address, int pageCount)
	{
		int num = 4096 * pageCount;
		IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(num);
		Span<byte> span = memoryOwner.Memory.Slice(0, num).Span;
		if (!_pageBackend.TryReadMemory(address, span))
		{
			memoryOwner.Dispose();
			return null;
		}
		return memoryOwner;
	}

	public bool TryReadMemory(IntPtr address, Span<byte> target)
	{
		_lock.EnterReadLock();
		try
		{
			if (target.Length == 0)
			{
				return true;
			}
			IntPtr pageAddress = GetPageAddress(address);
			IntPtr pageAddress2 = GetPageAddress((nint)address + target.Length - 1);
			if (pageAddress.GetValue() > pageAddress2.GetValue())
			{
				return false;
			}
			int num = checked((int)(address.GetValue() - pageAddress.GetValue()));
			nint num2 = pageAddress;
			Memory<byte> memory;
			Span<byte> span;
			while (num2.GetValue() < pageAddress2.GetValue())
			{
				IMemoryOwner<byte> page = GetPage(num2);
				if (page == null)
				{
					return false;
				}
				memory = page.Memory;
				span = memory.Span;
				int num3 = num;
				span.Slice(num3, 4096 - num3).CopyTo(target);
				num2 += 4096;
				num3 = 4096 - num;
				target = target.Slice(num3, target.Length - num3);
				num = 0;
			}
			IMemoryOwner<byte> page2 = GetPage(pageAddress2);
			if (page2 == null)
			{
				return false;
			}
			memory = page2.Memory;
			span = memory.Span;
			span.Slice(num, target.Length).CopyTo(target);
		}
		finally
		{
			_lock.ExitReadLock();
		}
		return true;
	}

	private IMemoryOwner<byte> GetPage(IntPtr pageAddress)
	{
		IMemoryOwner<byte> orAdd = _cachedPages.GetOrAdd(pageAddress, GetRealPage);
		_pagesRequestedOnLastIteration.TryAdd(pageAddress, orAdd != null);
		return orAdd;
	}

	public void NotifyFrame()
	{
		_lock.EnterWriteLock();
		try
		{
			_nextFrameCts?.Cancel();
			CancellationTokenSource thisFrameCts = new CancellationTokenSource();
			_nextFrameCts = thisFrameCts;
			_pageBackend.NotifyFrame();
			List<(IntPtr, int)> pagesToPreRead = (from x in (from x in _pagesRequestedOnLastIteration
					where x.Value
					select x.Key into x
					orderby x
					select x).Segment((IntPtr b, IntPtr a, int _) => b.GetValue() - a.GetValue() != 4096)
				select (x.First(), x.Count())).ToList();
			_pagesRequestedOnLastIteration.Clear();
			DropRentedPages();
			Task.Run(delegate
			{
				foreach (var (num, num2) in pagesToPreRead)
				{
					if (thisFrameCts.IsCancellationRequested)
					{
						break;
					}
					using IMemoryOwner<byte> memoryOwner = GetRealPage(num, num2);
					if (thisFrameCts.IsCancellationRequested)
					{
						break;
					}
					if (memoryOwner != null)
					{
						for (int i = 0; i < num2 * 4096; i += 4096)
						{
							IMemoryOwner<byte> memoryOwner2 = MemoryPool<byte>.Shared.Rent(4096);
							memoryOwner.Memory.Slice(i, 4096).CopyTo(memoryOwner2.Memory.Slice(0, 4096));
							_lock.EnterReadLock();
							try
							{
								if (_disposed || !_cachedPages.TryAdd(num + i, memoryOwner2))
								{
									memoryOwner2.Dispose();
								}
							}
							finally
							{
								_lock.ExitReadLock();
							}
						}
					}
				}
			}, thisFrameCts.Token);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	private void DropRentedPages()
	{
		foreach (KeyValuePair<IntPtr, IMemoryOwner<byte>> cachedPage in _cachedPages)
		{
			if (_cachedPages.TryRemove(cachedPage))
			{
				cachedPage.Value?.Dispose();
			}
		}
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}
		_lock.EnterWriteLock();
		try
		{
			if (!_disposed)
			{
				_disposed = true;
				_nextFrameCts?.Cancel();
				DropRentedPages();
			}
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}
}
