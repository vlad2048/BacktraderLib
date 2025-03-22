using BacktraderLib._sys;
using BaseUtils;
using Frames;

namespace BacktraderLib;


public static class Indicators
{
	public static Frame<string, string> SMA(this Frame<string, string> frame, int period) => frame.MapValues(xs => SMA(xs, period)).WithName("SMA");
	public static Frame<string, string> SMA(this Frame<string, string, Bar> frame, int period) => frame.Get(Bar.Close).MapValues(xs => SMA(xs, period)).WithName("SMA");
	static double[] SMA(this double[] xsSrc, int period)
	{
		if (period < 2) throw new ArgumentException($"SMA period too small ({period} < 2)");
		return xsSrc.Apply(TALib.Functions.Sma, TALib.Functions.SmaLookback(period), period);
	}

	public static Frame<string, string> RSI(this Frame<string, string> frame, int period) => frame.MapValues(xs => RSI(xs, period)).WithName("RSI");
	public static Frame<string, string> RSI(this Frame<string, string, Bar> frame, int period) => frame.Get(Bar.Close).MapValues(xs => RSI(xs, period)).WithName("RSI");
	static double[] RSI(this double[] xsSrc, int period)
	{
		//if (period < 2) throw new ArgumentException($"RSI period too small ({period} < 2)");
		return xsSrc.Apply(TALib.Functions.Rsi, TALib.Functions.RsiLookback(period), period);
	}



	delegate TALib.Core.RetCode TAFun(
		ReadOnlySpan<double> inReal,
		Range inRange,
		Span<double> outReal,
		out Range outRange,
		int period
	);


	static double[] Apply(this double[] xsSrc, TAFun fun, int look, int period)
	{
		var xsDst = MakeArray(xsSrc.Length);
		var xsDstSpan = xsDst.AsSpan();

		var ranges = xsSrc.GetRanges().WhereA(e => e.Length() > look);

		foreach (var range in ranges)
		{
			fun(
				xsSrc,
				MakeRange(range.Start.Value + look, range.End.Value - 1),
				xsDstSpan[(range.Start.Value + look)..],
				out _,
				period
			).Check();
		}

		return xsDst;
	}

	static Range MakeRange(int idxStart, int idxEnd) => new(new Index(idxStart), new Index(idxEnd));

	static int Length(this Range range) =>
		(range.Start.IsFromEnd, range.End.IsFromEnd) switch
		{
			(false, false) => range.End.Value - range.Start.Value,
			_ => throw new ArgumentException("Invalid range"),
		};



	static Range[] GetRanges(this double[] xs)
	{
		var list = new List<Range>();
		Index? min = null;
		for (var i = 0; i < xs.Length; i++)
		{
			switch (min, !xs[i].IsNaN())
			{
				case (null, true):
					min = new Index(i);
					break;
				case (null, false):
					break;

				case (not null, false):
					list.Add(new Range(min.Value, new Index(i)));
					min = null;
					break;

				case (not null, true):
					break;
			}
		}
		if (min is not null)
			list.Add(new Range(min.Value, xs.Length));
		return [.. list];
	}


	static double[] MakeArray(int n)
	{
		var xsDst = new double[n];
		Array.Fill(xsDst, double.NaN);
		return xsDst;
	}

	static void Check(this TALib.Core.RetCode res)
	{
		if (res != TALib.Core.RetCode.Success) throw new ArgumentException($"{res}");
	}
}
