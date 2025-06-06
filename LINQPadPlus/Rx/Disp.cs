﻿using System.Collections;
using System.Reactive.Disposables;

namespace LINQPadPlus.Rx;

/// <summary>
/// Represents a group of disposable resources that are disposed together.
/// </summary>
public sealed class Disp : ICollection<IDisposable>, ICancelable
{
	const bool DisposeInReverseOrder = true;
	
	readonly object _gate = new();
	bool _disposed;
	List<IDisposable?> _disposables;
	int _count;
	const int ShrinkThreshold = 64;

	public override string ToString() => $"Disp(cnt:{Count})";

	/// <summary>
	/// Initializes a new instance of the <see cref="Disp"/> class with no disposables contained by it initially.
	/// </summary>
	public Disp()
	{
		_disposables = [];
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Disp"/> class with the specified number of disposables.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
	public Disp(int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(capacity));
		}

		_disposables = new List<IDisposable?>(capacity);
	}

	/// <summary>
	/// Gets the number of disposables contained in the <see cref="Disp"/>.
	/// </summary>
	public int Count => Volatile.Read(ref _count);

	/// <summary>
	/// Adds a disposable to the <see cref="Disp"/> or disposes the disposable if the <see cref="Disp"/> is disposed.
	/// </summary>
	/// <param name="item">Disposable to add.</param>
	/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
	public void Add(IDisposable item)
	{
		if (item == null)
		{
			throw new ArgumentNullException(nameof(item));
		}

		lock (_gate)
		{
			if (!_disposed)
			{
				_disposables.Add(item);

				// If read atomically outside the lock, it should be written atomically inside
				// the plain read on _count is fine here because manipulation always happens
				// from inside a lock.
				Volatile.Write(ref _count, _count + 1);
				return;
			}
		}

		item.Dispose();
	}

	/// <summary>
	/// Removes and disposes the first occurrence of a disposable from the <see cref="Disp"/>.
	/// </summary>
	/// <param name="item">Disposable to remove.</param>
	/// <returns>true if found; false otherwise.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
	public bool Remove(IDisposable item)
	{
		if (item == null)
		{
			throw new ArgumentNullException(nameof(item));
		}

		lock (_gate)
		{
			// this composite was already disposed and if the item was in there
			// it has been already removed/disposed
			if (_disposed)
			{
				return false;
			}

			//
			// List<T> doesn't shrink the size of the underlying array but does collapse the array
			// by copying the tail one position to the left of the removal index. We don't need
			// index-based lookup but only ordering for sequential disposal. So, instead of spending
			// cycles on the Array.Copy imposed by Remove, we use a null sentinel value. We also
			// do manual Swiss cheese detection to shrink the list if there's a lot of holes in it.
			//

			// read fields as infrequently as possible
			var current = _disposables;

			var i = current.IndexOf(item);
			if (i < 0)
			{
				// not found, just return
				return false;
			}

			current[i] = null;

			if (current.Capacity > ShrinkThreshold && _count < current.Capacity / 2)
			{
				var fresh = new List<IDisposable?>(current.Capacity / 2);

				foreach (var d in current)
				{
					if (d != null)
					{
						fresh.Add(d);
					}
				}

				_disposables = fresh;
			}

			// make sure the Count property sees an atomic update
			Volatile.Write(ref _count, _count - 1);
		}

		// if we get here, the item was found and removed from the list
		// just dispose it and report success

		item.Dispose();

		return true;
	}

	/// <summary>
	/// Disposes all disposables in the group and removes them from the group.
	/// </summary>
	public void Dispose()
	{
		List<IDisposable?>? currentDisposables = null;

		lock (_gate)
		{
			if (!_disposed)
			{
				currentDisposables = _disposables;

				// nulling out the reference is faster no risk to
				// future Add/Remove because _disposed will be true
				// and thus _disposables won't be touched again.
				_disposables = null!; // NB: All accesses are guarded by _disposed checks.

				Volatile.Write(ref _count, 0);
				Volatile.Write(ref _disposed, true);
			}
		}

		if (currentDisposables != null)
		{
			if (DisposeInReverseOrder)
			{
				foreach (var d in Enumerable.Reverse(currentDisposables))
				{
					//var tname = d?.GetType().Name!;
					//if (!tname.Contains("LiteSubject") && !tname.Contains("Anonymous"))
					//	Console.WriteLine($"[{Name}].Dispose( {tname} )");
					d?.Dispose();
				}
			}
			else
			{
				foreach (var d in currentDisposables)
				{
					d?.Dispose();
				}
			}
		}
	}

	/// <summary>
	/// Removes and disposes all disposables from the <see cref="Disp"/>, but does not dispose the <see cref="Disp"/>.
	/// </summary>
	public void Clear()
	{
		IDisposable?[] previousDisposables;

		lock (_gate)
		{
			// disposed composites are always clear
			if (_disposed)
			{
				return;
			}

			var current = _disposables;

			previousDisposables = current.ToArray();
			current.Clear();

			Volatile.Write(ref _count, 0);
		}

		if (DisposeInReverseOrder)
		{
			foreach (var d in Enumerable.Reverse(previousDisposables))
			{
				d?.Dispose();
			}
		}
		else
		{
			foreach (var d in previousDisposables)
			{
				d?.Dispose();
			}
		}
	}

	/// <summary>
	/// Determines whether the <see cref="Disp"/> contains a specific disposable.
	/// </summary>
	/// <param name="item">Disposable to search for.</param>
	/// <returns>true if the disposable was found; otherwise, false.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
	public bool Contains(IDisposable item)
	{
		if (item == null)
		{
			throw new ArgumentNullException(nameof(item));
		}

		lock (_gate)
		{
			if (_disposed)
			{
				return false;
			}

			return _disposables.Contains(item);
		}
	}

	/// <summary>
	/// Copies the disposables contained in the <see cref="Disp"/> to an array, starting at a particular array index.
	/// </summary>
	/// <param name="array">Array to copy the contained disposables to.</param>
	/// <param name="arrayIndex">Target index at which to copy the first disposable of the group.</param>
	/// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero. -or - <paramref name="arrayIndex"/> is larger than or equal to the array length.</exception>
	public void CopyTo(IDisposable[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		if (arrayIndex < 0 || arrayIndex >= array.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		}

		lock (_gate)
		{
			// disposed composites are always empty
			if (_disposed)
			{
				return;
			}

			if (arrayIndex + _count > array.Length)
			{
				// there is not enough space beyond arrayIndex 
				// to accommodate all _count disposables in this composite
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			}

			var i = arrayIndex;

			foreach (var d in _disposables)
			{
				if (d != null)
				{
					array[i++] = d;
				}
			}
		}
	}

	/// <summary>
	/// Always returns false.
	/// </summary>
	public bool IsReadOnly => false;

	/// <summary>
	/// Returns an enumerator that iterates through the <see cref="Disp"/>.
	/// </summary>
	/// <returns>An enumerator to iterate over the disposables.</returns>
	public IEnumerator<IDisposable> GetEnumerator()
	{
		lock (_gate)
		{
			if (_disposed || _count == 0)
			{
				return EmptyEnumerator;
			}

			// the copy is unavoidable but the creation
			// of an outer IEnumerable is avoidable
			return new CompositeEnumerator(_disposables.ToArray());
		}
	}

	/// <summary>
	/// Returns an enumerator that iterates through the <see cref="Disp"/>.
	/// </summary>
	/// <returns>An enumerator to iterate over the disposables.</returns>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	/// Gets a value that indicates whether the object is disposed.
	/// </summary>
	public bool IsDisposed => Volatile.Read(ref _disposed);

	/// <summary>
	/// An empty enumerator for the <see cref="GetEnumerator"/>
	/// method to avoid allocation on disposed or empty composites.
	/// </summary>
	static readonly CompositeEnumerator EmptyEnumerator =
		new([]);

	/// <summary>
	/// An enumerator for an array of disposables.
	/// </summary>
	sealed class CompositeEnumerator : IEnumerator<IDisposable>
	{
		readonly IDisposable?[] _disposables;
		int _index;

		public CompositeEnumerator(IDisposable?[] disposables)
		{
			_disposables = disposables;
			_index = -1;
		}

		public IDisposable Current => _disposables[_index]!; // NB: _index is only advanced to non-null positions.

		object IEnumerator.Current => _disposables[_index]!;

		public void Dispose()
		{
			// Avoid retention of the referenced disposables
			// beyond the lifecycle of the enumerator.
			// Not sure if this happens by default to
			// generic array enumerators though.
			var disposables = _disposables;
			Array.Clear(disposables, 0, disposables.Length);
		}

		public bool MoveNext()
		{
			var disposables = _disposables;

			for (; ; )
			{
				var idx = ++_index;

				if (idx >= disposables.Length)
				{
					return false;
				}

				// inlined that filter for null elements
				if (disposables[idx] != null)
				{
					return true;
				}
			}
		}

		public void Reset()
		{
			_index = -1;
		}
	}
}
