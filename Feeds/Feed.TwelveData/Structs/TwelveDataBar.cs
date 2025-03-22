namespace Feed.TwelveData;

public sealed record TwelveDataBar(
	DateTime Date,
	double Open,
	double High,
	double Low,
	double Close,
	double Volume
);