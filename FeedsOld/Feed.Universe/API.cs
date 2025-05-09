using BaseUtils;
using Feed.Universe._sys;

namespace Feed.Universe;

public static class API
{
	public static void SetTwelveDataApiKey(string apiKey) => TwelveDataSymbolsGetter.apiKey = apiKey;

	public static UniverseSymbol[] LoadUniverse(IUniverse universe) => UniverseConstituentCleaner.GetSymbols(universe);

	public static CompanyDef[] Companies => companies.Value;

	public static TwelveDataSymbol[] LoadAllTwelveDataSymbols() => TwelveDataSymbolsGetter.All;


	static readonly Lazy<CompanyDef[]> companies = new(CompanyDefsCacher.Load);
}



public static class APIDev
{
	public static StockAnalysisSymbol[] GetStockAnalysisUniverseSymbols(IUniverse universe) => StockAnalysisUniverseSymbolsGetter.Scrape(universe);

	public static TwelveDataSymbol[] TwelveDataSymbols => TwelveDataSymbolsGetter.All;
}