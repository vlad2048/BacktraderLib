namespace Feed.Symbology;

public sealed record TwelveDataSymbol(
	string Symbol,
	string Exchange,
	string Country,
	string Name,
	string Currency,
	string MicCode,
	string Type,
	string FigiCode,
	string CfiCode
);