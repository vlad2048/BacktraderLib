﻿using System.Collections;
using BaseUtils;
using Frames._sys;
using Frames._sys.Utils;
using PrettyPrinting;

namespace Frames;

public static class Serie
{
	public static Serie<N> Make<N>(
		N name,
		DateTime[] index,
		double[] data
	) => new(
		name,
		index,
		data
	);
}

public static class Frame
{
	public static Frame<N, K1> Make<N, K1>(
		N name,
		DateTime[] index,
		(K1, double[])[] data
	) => new(
		name,
		index,
		data
	);

	public static Frame<N, K1, K2> Make<N, K1, K2>(
		N name,
		DateTime[] index,
		(K1, (K2, double[])[])[] data
	) => new(
		name,
		index,
		data
	);

	public static void Dbg_SetFramePrinterCutoffs(int rowMax, int rowSmp, int colMax, int colSmp) => (FramePrinter.RowMax, FramePrinter.RowSmp, FramePrinter.ColMax, FramePrinter.ColSmp) = (rowMax, rowSmp, colMax, colSmp);
}



// ************
// * Serie<N> *
// ************
public sealed class Serie<N> : ITxt
{
	internal Serie(N name, DateTime[] index, double[] data)
	{
		Name = name;
		Index = index;
		Values = data;
		if (Index.Length == 0 || Index.Length != Values.Length) throw new ArgumentException("Invalid Serie lengths");
	}

	// Public
	// ------
	public N Name { get; }
	public DateTime[] Index { get; }
	public int RowCount => Index.Length;
	public double[] Values { get; }

	// Printing
	// --------
	public object ToDump() => Txt;
	public TxtArray Txt => FramePrinter.Print(this);
	public override string ToString() => $"Serie[{Name}](rows:{RowCount})";
}



// ****************
// * Frame<N, K1> *
// ****************
public sealed class Frame<N, K1> : IEnumerable<Serie<K1>>, ITxt
{
	readonly (K1, double[])[] data;
	readonly IReadOnlyDictionary<K1, Lazy<Serie<K1>>> mapF;

	internal Frame(N name, DateTime[] index, (K1, double[])[] data)
	{
		Check.SameNumberOfRows(index.Length, data.Select(e => e.Item2));
		Check.AtLeastOneColumn(data);

		Name = name;
		Index = index;
		this.data = data;
		mapF = data.ToOrderedDictionary(e => e.Item1, e => Lazy.Make(() => Serie.Make(e.Item1, Index, e.Item2)));
	}

	// Public
	// ------
	public N Name { get; }
	public DateTime[] Index { get; }
	public int ColCount => data.Length;
	public int RowCount => Index.Length;
	public Serie<K1> this[K1 key] => mapF[key].Value;

	// IEnumerable<Serie<K1>>
	// ----------------------
	public IEnumerator<Serie<K1>> GetEnumerator() => mapF.Values.Select(e => e.Value).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	// Printing
	// --------
	public object ToDump() => Txt;
	public TxtArray Txt => FramePrinter.Print(this);
	public override string ToString() => $"Frame<{typeof(K1).Name}>[{Name}](cols:{ColCount} rows:{RowCount})";
}



// ********************
// * Frame<N, K1, K2> *
// ********************
public sealed class Frame<N, K1, K2> : IEnumerable<Frame<K1, K2>>, ITxt
{
	readonly (K1, (K2, double[])[])[] data;
	readonly IReadOnlyDictionary<K1, Lazy<Frame<K1, K2>>> mapF;

	internal Frame(N name, DateTime[] index, (K1, (K2, double[])[])[] data)
	{
		Check.SameNumberOfRows(index.Length, data.SelectMany(e => e.Item2.Select(f => f.Item2)));
		Check.AtLeastOneColumn(data);

		Name = name;
		Index = index;
		this.data = data;
		mapF = data.ToOrderedDictionary(e => e.Item1, e => Lazy.Make(() => Frame.Make(e.Item1, Index, e.Item2)));
	}

	// Public
	// ------
	public N Name { get; }
	public DateTime[] Index { get; }
	public int ColCount => data.Length;
	public int RowCount => Index.Length;
	public Frame<K1, K2> this[K1 key1] => mapF[key1].Value;
	public Serie<K2> this[K1 key1, K2 key2] => mapF[key1].Value[key2];
	public Frame<N, K1> Get(K2 subColumn)
		=> Frame.Make(
			Name,
			Index,
			this
				.Where(e => e.Any(f => Equals(f.Name, subColumn)))
				.SelectA(e => (
					e.Name,
					e[subColumn].Values
				))
		);

	// IEnumerable<Frame<K1, K2>>
	// --------------------------
	public IEnumerator<Frame<K1, K2>> GetEnumerator() => mapF.Values.Select(e => e.Value).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	// Printing
	// --------
	public object ToDump() => Txt;
	public TxtArray Txt => FramePrinter.Print(this);
	public override string ToString() => $"Frame<{typeof(K1).Name}, {typeof(K2).Name}>[{Name}](cols:{ColCount} rows:{RowCount})";
}





// ******************
// * Internal utils *
// ******************
file static class Check
{
	public static void SameNumberOfRows(int n, IEnumerable<double[]> xs)
	{
		if (xs.Any(x => x.Length != n)) throw new ArgumentException("Different number of rows");
	}

	// ReSharper disable once SuggestBaseTypeForParameter
	public static void AtLeastOneColumn<K1>((K1, double[])[] data)
	{
		if (data.Length == 0) throw new ArgumentException("No columns");
	}

	// ReSharper disable once SuggestBaseTypeForParameter
	public static void AtLeastOneColumn<K1, K2>((K1, (K2, double[])[])[] data)
	{
		if (data.Length == 0) throw new ArgumentException("No columns");
	}
}
