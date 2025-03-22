using BaseUtils;
using Feed.NoPrefs._sys.DoltLogic;

namespace Feed.NoPrefs._sys;

static class Keepers
{
	public static Keeper<IReadOnlyDictionary<string, Symbol>> Symbols = new(
		Path.Combine(Consts.DataFolder, "symbols.json"),
		() => new Dictionary<string, Symbol>(),
		(db, _) => db.Read<Symbol>("SELECT * FROM symbol").ToDictionary(e => e.ActSymbol)
	);

	public static Keeper<Price> Price(string symbol) => new(
		Path.Combine(
			Path.Combine(Consts.DataFolder, symbol.GetPrefix()),
			$"{symbol}.json"
		),
		() => new Price(symbol, [], Array.Empty<PriceDividend>(), Array.Empty<PriceSplit>()),
		(db, prev) =>
		{
			var barsT = prev.Bars.GetLast(e => e.Date);
			var divsT = prev.Dividends.GetLast(e => e.ExDate);
			var spltT = prev.Splits.GetLast(e => e.ExDate);

			// @formatter:off
			var bars = db.Read<NoPrefsBar>($"""
				SELECT
					date, open, high, low, close, volume
				FROM
					ohlcv
				WHERE
					act_symbol = '{symbol}'      AND
					date       > '{barsT.Fmt()}'
				ORDER BY
					date
				"""
			);

			var dividends = db.Read<PriceDividend>($"""
				SELECT
					ex_date, amount
				FROM
					dividend
				WHERE
					act_symbol = '{symbol}'      AND
					ex_date    > '{divsT.Fmt()}'
				ORDER BY
					ex_date
				"""
			);

			var splits = db.Read<PriceSplit>($"""
				SELECT
					ex_date, to_factor, for_factor
				FROM
					split
				WHERE
					act_symbol = '{symbol}'      AND
					ex_date    > '{spltT.Fmt()}'
				ORDER BY
					ex_date
				"""
			);
			// @formatter:on

			var next = new Price(symbol, bars, dividends, splits);

			return PriceUtils.Merge(prev, next).SanityCheck();
		}
	);

	

	static DateTime GetLast<T>(this T[] arr, Func<T, DateTime> fun) =>
		arr.Length switch
		{
			0 => new DateTime(1900, 1, 1),
			_ => fun(arr[^1]),
		};

	static string GetPrefix(this string e) => e[..Math.Min(1, e.Length)];

	static string Fmt(this DateTime e) => $"{e:yyyy-MM-dd HH:mm:ss}";
}




file static class PriceUtils
{
	public static Price Merge(Price prev, Price next) => new(
		prev.Symbol,
		prev.Bars.Concat(next.Bars).ToArray(),
		prev.Dividends.Concat(next.Dividends).ToArray(),
		prev.Splits.Concat(next.Splits).ToArray()
	);

	public static Price SanityCheck(this Price price)
	{
		var n = price.Bars.Length;

		var dates = price.Bars.SelectA(e => e.Date.Date).Distinct().ToArray();
		if (dates.Length != n) throw new ArgumentException("Non unique dates");

		return price;
	}
}