namespace Feed.YahooOoples;

public sealed record OoplesPrice(
	string Symbol,
	OoplesBar[] Bars,
	OoplesDividend[] Dividends,
	OoplesSplit[] Splits
);

public sealed record OoplesBar(
	DateTime Date,
	double Open,
	double High,
	double Low,
	double Close,
	double Volume
);

public sealed record OoplesDividend(
	DateTime ExDate,
	double Amount
);

public sealed record OoplesSplit(
	DateTime ExDate,
	double ToFactor,
	double ForFactor
)
{
	public override string ToString() => $"[{ExDate:yyyy-MM-dd}] {ToFactor / ForFactor:F4}";
}
