namespace Feed.Yahoo;

public sealed record YahooBar(
	DateTime Date,
	double Open,
	double High,
	double Low,
	double Close,
	double Volume
);