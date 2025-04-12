﻿using BaseUtils;

namespace Feed.Universe._sys;

public static class UniverseConstituentCleaner
{
	public static UniverseSymbol[] GetSymbols(IUniverse universe)
	{
		var symNames = CompanyMatcher.GetSymNames();
		var (secNames, secDisambiguator) = CompanyMatcher.GetSecNamesAndDisambiguator();
		var companyMatches = CompanyMatcher.Match(symNames, secNames, secDisambiguator);

		var tups = GetSymbolsInternal(universe);
		return tups
			.Where(t => companyMatches.IsMapped(t.Item2.Name))
			.SelectA(t =>
			{
				var e = t.Item1;
				var f = t.Item2;
				return new UniverseSymbol(
					f.Symbol,
					f.Country,
					f.Name,
					f.Currency,
					f.Exchange,
					f.MicCode,
					f.Type,
					f.FigiCode,
					f.CfiCode,

					companyMatches.Map(f.Name),

					e.MarketCap,
					e.Revenue
				);
			});
	}

	public static (StockAnalysisSymbol, TwelveDataSymbol)[] GetSymbolsInternal(IUniverse universe)
	{
		var exchanges = universe.GetExchanges().ToHashSet(e => $"{e}");
		var countries = universe.GetCountries().ToHashSet();

		var twelvedataSymbols = TwelveDataSymbolsGetter.All
			.Where(e => e.Type == "Common Stock")
			.Where(e => exchanges.Contains(e.Exchange))
			.Where(e => countries.Contains(e.Country))
			.ToDictionary(e => e.Symbol);

		var stockanalysisSymbols = StockAnalysisUniverseSymbolsGetter.Scrape(universe);

		var tups = stockanalysisSymbols
			.Where(e => twelvedataSymbols.ContainsKey(e.Symbol))
			.SelectA(e => (e, twelvedataSymbols[e.Symbol]));

		return tups;

		/*return stockanalysisSymbols
			.Where(e => twelvedataSymbols.ContainsKey(e.Symbol))
			.SelectA(e =>
			{
				var f = twelvedataSymbols[e.Symbol];
				return new UniverseSymbol(
					f.Symbol,
					f.Country,
					f.Name,
					f.Currency,
					f.Exchange,
					f.MicCode,
					f.Type,
					f.FigiCode,
					f.CfiCode,

					e.MarketCap,
					e.Revenue
				);
			});*/
	}
}