using BaseUtils;
using Feed.NoPrefs._sys.DoltLogic;

namespace Feed.NoPrefs._sys;

static class Keepers
{
	public static readonly Keeper<IReadOnlyDictionary<string, Symbol>> Symbols = new(
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


	public static readonly Keeper<Yields> Yields = new(
		Path.Combine(Consts.DataFolder, "yields.json"),
		() => new Yields(Array.Empty<DateTime>(), new SortedDictionary<int, decimal?[]>()),
		(db, prev) =>
		{
			var lastT = prev.Index.GetLast(e => e);

			// @formatter:off
			var data = db.Read<YieldsRow>($"""
				SELECT
					date, 1_month, 2_month, 3_month, 6_month, 1_year, 2_year, 3_year, 5_year, 7_year, 10_year, 20_year, 30_year
				FROM
					us_treasury
				WHERE
					date > '{lastT.Fmt()}'
				ORDER BY
					date
				"""
			);
			// @formatter:on

			var next = new Yields(
				data.Select(e => e.Date).ToArray(),
				new SortedDictionary<int, decimal?[]>
				{
					{ 1, data.SelectA(e => e._1_Month) },
					{ 2, data.SelectA(e => e._2_Month) },
					{ 3, data.SelectA(e => e._3_Month) },
					{ 6, data.SelectA(e => e._6_Month) },
					{ 12, data.SelectA(e => e._1_Year) },
					{ 24, data.SelectA(e => e._2_Year) },
					{ 36, data.SelectA(e => e._3_Year) },
					{ 60, data.SelectA(e => e._5_Year) },
					{ 84, data.SelectA(e => e._7_Year) },
					{ 120, data.SelectA(e => e._10_Year) },
					{ 240, data.SelectA(e => e._20_Year) },
					{ 360, data.SelectA(e => e._30_Year) },
				}
			);

			return YieldsUtils.Merge(prev, next).SanityCheck();
		}
	);



	// ReSharper disable once ClassNeverInstantiated.Local
	sealed record YieldsRow(
		DateTime Date,
		decimal? _1_Month,
		decimal? _2_Month,
		decimal? _3_Month,
		decimal? _6_Month,
		decimal? _1_Year,
		decimal? _2_Year,
		decimal? _3_Year,
		decimal? _5_Year,
		decimal? _7_Year,
		decimal? _10_Year,
		decimal? _20_Year,
		decimal? _30_Year
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