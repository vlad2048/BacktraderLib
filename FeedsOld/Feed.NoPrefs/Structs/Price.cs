using BaseUtils;

namespace Feed.NoPrefs;

public sealed record Price(
	string Symbol,
	NoPrefsBar[] Bars,
	PriceDividend[] Dividends,
	PriceSplit[] Splits
);

public sealed record NoPrefsBar(
	DateTime Date,
	decimal Open,
	decimal High,
	decimal Low,
	decimal Close,
	long Volume
);

public sealed record PriceDividend(
	DateTime ExDate,
	decimal Amount
);

public sealed record PriceSplit(
	DateTime ExDate,
	decimal ToFactor,
	decimal ForFactor
)
{
	public override string ToString() => $"[{ExDate:yyyy-MM-dd}] {ToFactor/ForFactor:F4}";
}



static class PriceUtils
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