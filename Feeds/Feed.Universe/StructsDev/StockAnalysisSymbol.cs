namespace Feed.Universe;

public sealed record StockAnalysisSymbol(
	string Symbol,
	decimal MarketCap,
	decimal Revenue
);