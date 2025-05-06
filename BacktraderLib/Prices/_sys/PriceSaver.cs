using BaseUtils;
using Frames;

namespace BacktraderLib._sys;

static class PriceSaver
{
	public static Serie<string> SaveSerie(Serie<string> serie, string file)
	{
		var df = Frame.Make(
			serie.Name,
			serie.Index,
			[(serie.Name, serie.Values)]
		);
		SaveFrame(df, file);
		return serie;
	}

	
	
	public static Frame<string, string> SaveFrame(Frame<string, string> df, string file)
	{
		using var fs = new FileStream(file, FileMode.Create);
		using var sw = new StreamWriter(fs);
		sw.WriteLine(df.Select(e => e.Name).Prepend("date").JoinText(","));
		for (var t = 0; t < df.Index.Length; t++)
		{
			var date = df.Index[t];
			sw.Write($"{date.f()}");
			foreach (var col in df)
				sw.Write($",{col.Values[t].f()}");
			sw.WriteLine();
		}
		return df;
	}



	public static Frame<string, string, Bar> SaveFrame2(Frame<string, string, Bar> df, string file)
	{
		df.Flatten().SaveFrame(file);
		return df;
	}


	static Frame<string, string> Flatten(this Frame<string, string, Bar> df) =>
		Frame.Make(
			df.Name,
			df.Index,
			(
				from col in df
				from subCol in col
				select (
					$"{col.Name}_{subCol.Name}",
					subCol.Values
				)
			)
			.ToArray()
		);




	/*
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
	*/

	// TODO (same as in MarketDataLib\_Internal\Utils\TickSaverLoader.cs)
	static string f(this DateTime v) =>
		(v.TimeOfDay == TimeSpan.Zero) switch
		{
			true => $"{v:yyyy-MM-dd}",
			false => $"{v:yyyy-MM-dd HH:mm:ss}",
		};

	static string f(this double v) => $"{v}";
}