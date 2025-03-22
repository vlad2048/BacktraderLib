namespace Feed.TwelveData._sys.Structs;

sealed record Meta(
	string Symbol,
	Freq Interval,
	string Currency,
	string ExchangeTimezone,
	string Exchange,
	string MicCode,
	SymbolType Type
);