namespace Feed.TwelveData._sys.Structs;

sealed record TwelveDataBar(
	DateTime Datetime,
	double Open,
	double High,
	double Low,
	double Close,
	long Volume
);