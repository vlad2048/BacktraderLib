using Feed.Universe._sys;

namespace Feed.Universe;

public static class API
{
	public static void SetTwelveDataApiKey(string apiKey) => TwelveDataSymbolsGetter.ApiKey = apiKey;

	public static UniverseSymbol[] GetUniverseConstituents(IUniverse universe) => UniverseConstituentCleaner.GetSymbols(universe);
}



public static class APIDev
{
	public static StockAnalysisSymbol[] GetStockAnalysisUniverseSymbols(IUniverse universe) => StockAnalysisUniverseSymbolsGetter.Scrape(universe);

	public static TwelveDataSymbol[] TwelveDataSymbols => TwelveDataSymbolsGetter.All;
}