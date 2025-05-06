using BaseUtils;

namespace Feed.Universe;

static class CleanLogic
{
	public static TwelveDataSymbol[] CleanTwelveDataSymbols(this IEnumerable<TwelveDataSymbol> source) =>
		source
			.Where(e => e.Type == "Common Stock")
			.Where(e => e.FigiCode != string.Empty)
			.Where(e => e.Symbol != "ARQQW")
			.Where(e => !forbiddenKeys.Contains(e.Key))
			.EnsureUniqueBy(e => e.Key, "TwelveDataSymbols should be unique by (Symbol,Exchange,Country)")
			.ToArray();



	static readonly HashSet<TwelveDataSymbolKey> forbiddenKeys =
	[
		new TwelveDataSymbolKey("ZG", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("BATRK", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("KELYB", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("UONEK", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("LBTYB", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("LBTYA", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("NWS", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("BIO.B", "NYSE", "United States"),
		new TwelveDataSymbolKey("HVT.A", "NYSE", "United States"),
		new TwelveDataSymbolKey("UHAL.B", "NYSE", "United States"),
	];
}