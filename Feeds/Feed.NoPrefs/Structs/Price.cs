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
