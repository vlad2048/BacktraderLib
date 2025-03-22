using BaseUtils;
using Frames;

namespace FeedUtils;

public static class PriceBuilder
{
	public static Frame<string, string, Bar> Build<B>(
		(string Name, B[] Bars)[] prices,
		Func<B, DateTime> dateFun,
		Func<B, double> openFun,
		Func<B, double> highFun,
		Func<B, double> lowFun,
		Func<B, double> closeFun,
		Func<B, double> volumeFun
	)
	{
		var xss = prices.SelectA(e => new SortedSet<DateTime>(e.Bars.SelectA(dateFun)));
		var align = DateAligner.Align(xss);
		var index = align.Keys.ToArray();
		var map = prices.SelectA(e => Load(e.Name, e.Bars, dateFun, openFun, highFun, lowFun, closeFun, volumeFun, align));
		var arr = map.SelectA(price => (price.Name, price.Data));
		return Frame.Make("Prices", index, arr);

	}


	static (string Name, (Bar, double[])[] Data) Load<B>(
		string name,
		B[] bars,
		Func<B, DateTime> dateFun,
		Func<B, double> openFun,
		Func<B, double> highFun,
		Func<B, double> lowFun,
		Func<B, double> closeFun,
		Func<B, double> volumeFun,
		IReadOnlyDictionary<DateTime, int> align
	)
	{
		var n = align.Count;
		var (open, high, low, close, volume) = (new double[n], new double[n], new double[n], new double[n], new double[n]);
		Array.Fill(open, double.NaN);
		Array.Fill(high, double.NaN);
		Array.Fill(low, double.NaN);
		Array.Fill(close, double.NaN);
		Array.Fill(volume, double.NaN);

		foreach (var bar in bars)
		{
			var date = dateFun(bar);
			var idx = align[date];
			open[idx] = openFun(bar);
			high[idx] = highFun(bar);
			low[idx] = lowFun(bar);
			close[idx] = closeFun(bar);
			volume[idx] = volumeFun(bar);
		}

		return (
			name,
			[
				(Bar.Open, open),
				(Bar.High, high),
				(Bar.Low, low),
				(Bar.Close, close),
				(Bar.Volume, volume),
			]
		);
	}
}