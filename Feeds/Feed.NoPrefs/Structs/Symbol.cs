namespace Feed.NoPrefs;

public sealed record Symbol(
	string ActSymbol,
	string SecurityName,
	string ListingExchange,
	string? MarketCategory,
	bool? IsEtf,
	int RoundLotSize,
	bool IsTestIssue,
	string? FinancialStatus,
	string? CqsSymbol,
	string NasdaqSymbol,
	bool IsNextShares,
	DateTime LastSeen
);