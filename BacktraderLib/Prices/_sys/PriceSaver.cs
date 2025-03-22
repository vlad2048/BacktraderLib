using Frames;

namespace BacktraderLib._sys;

static class PriceSaver
{
	public static void Save(Frame<string, string, Bar> prices, string folder)
	{
		foreach (var price in prices)
			Save(price, Path.Combine(folder, $"{price.Name}.csv"));
	}

	static void Save(Frame<string, Bar> price, string file)
	{
		using var fs = new FileStream(file, FileMode.Create);
		using var sw = new StreamWriter(fs);
		sw.WriteLine("date,open,high,low,close,volume");
		for (var t = 0; t < price.Index.Length; t++)
		{
			var date = price.Index[t];
			var open = price[Bar.Open].Values[t];
			var high = price[Bar.High].Values[t];
			var low = price[Bar.Low].Values[t];
			var close = price[Bar.Close].Values[t];
			var volume = price[Bar.Volume].Values[t];

			if (open.IsNaN() || high.IsNaN() || low.IsNaN() || close.IsNaN() || volume.IsNaN()) continue;

			sw.Write($"{date.f()}");
			sw.Write($",{open.f()}");
			sw.Write($",{high.f()}");
			sw.Write($",{low.f()}");
			sw.Write($",{close.f()}");
			sw.Write($",{volume.f()}");
			sw.WriteLine();
		}
	}

	// TODO (same as in MarketDataLib\_Internal\Utils\TickSaverLoader.cs)
	static string f(this DateTime v) =>
		(v.TimeOfDay == TimeSpan.Zero) switch
		{
			true => $"{v:yyyy-MM-dd}",
			false => $"{v:yyyy-MM-dd HH:mm:ss}",
		};

	static string f(this double v) => $"{v}";
}