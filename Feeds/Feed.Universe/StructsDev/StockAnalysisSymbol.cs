namespace Feed.Universe;

public record StockAnalysisSymbol(
	string Symbol,
	decimal MarketCap,
	decimal Revenue
);