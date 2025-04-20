using BaseUtils;
using Feed.Universe._sys;

namespace Feed.Universe;

public static class API
{
	public static void SetTwelveDataApiKey(string apiKey) => TwelveDataSymbolsGetter.ApiKey = apiKey;

	public static UniverseSymbol[] LoadUniverse(IUniverse universe) => UniverseConstituentCleaner.GetSymbols(universe);

	public static CompanyDef[] LoadCompanies() => Universe.AllExchanges.SelectManyA(LoadUniverse)
		.Where(e => !e.Symbol.Contains("."))
		.GroupBy(e => e.SecCompanyName)
		.Select(e => new CompanyDef(
			e.Key,
			e.Select(e => e.Exchange).Distinct().Single(),
			e.MaxBy(f => f.MarketCap)!.Symbol,
			e.Max(f => f.MarketCap),
			e.Max(f => f.Revenue)
		))
		.OrderByDescending(e => e.MarketCap)
		.ToArray();
}



public static class APIDev
{
	public static StockAnalysisSymbol[] GetStockAnalysisUniverseSymbols(IUniverse universe) => StockAnalysisUniverseSymbolsGetter.Scrape(universe);

	public static TwelveDataSymbol[] TwelveDataSymbols => TwelveDataSymbolsGetter.All;
}