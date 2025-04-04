using BaseUtils;
using Feed.Trading212._sys.Structs;
using Feed.Trading212._sys.Utils;
using ScrapeUtils;

namespace Feed.Trading212._sys;

static class ScraperRunner
{
	public static async Task Scrape(SymbolDef[] symbols)
	{
		var Log = Logger.Make(LogCategory.ScrapeLoop);


		foreach (var symbol in symbols)
		{
			if (IsScrapeNeeded(symbol, out var done))
			{
				Log.BigTitle($"Scraping '{symbol}'");
				await InitPageIFN();
				var scrapeResult = await Scraper.Scrape(symbol, done);
				SaveScrapeResult(scrapeResult);
				Log($"Scraped {scrapeResult}");
			}
			else
			{
				Log($"'{symbol}' -> No scraping needed");
			}
		}
	}



	static async Task InitPageIFN()
	{
		if (FeedTrading212Setup.Page != null) return;
		FeedTrading212Setup.Page = await Web.Open(Consts.MainUrl);
	}

	static bool IsScrapeNeeded(SymbolDef symbol, out QuarterSet done)
	{
		var symbolFile = Consts.Data.CompanyJsonFile(symbol.SECCompany);
		done = new QuarterSet();
		if (!File.Exists(symbolFile))
		{
			return true;
		}
		else
		{
			var data = JsonUtils.Load<SymbolData>(symbolFile);
			if (data.ErrorMessage == null && DateTime.Now - data.ScrapeTime <= Consts.ScrapeFreq)
				return false;
			done = data.Reports.ToDictionary(e => e.Key, e => e.Value.Keys.ToHashSet());
			return true;
		}
	}

	static void SaveScrapeResult(ScrapeResult scrapeResult)
	{
		var symbolFile = Consts.Data.CompanyJsonFile(scrapeResult.Symbol.SECCompany);
		var reportsPrev = File.Exists(symbolFile) switch
		{
			true => JsonUtils.Load<SymbolData>(symbolFile).Reports,
			false => new Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>>(),
		};
		var symbolData = new SymbolData(
			scrapeResult.Symbol.SECCompany,
			DateTime.Now,
			scrapeResult.ErrorMessage,
			Merge(reportsPrev, scrapeResult.Reports)
		);
		symbolData.Save(symbolFile);
	}

	static Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Merge(Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> mapPrev, Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> mapNext)
	{
		var map = mapPrev.ToDictionary(kv => kv.Key, kv => kv.Value.ToSortedDictionary());
		foreach (var (reportType, quarterMapNext) in mapNext)
		{
			if (!map.TryGetValue(reportType, out var quarterMapPrev))
				quarterMapPrev = map[reportType] = new SortedDictionary<Quarter, RefField[]>();
			foreach (var (quarter, arr) in quarterMapNext)
				quarterMapPrev[quarter] = arr;
		}
		return map;
	}
}