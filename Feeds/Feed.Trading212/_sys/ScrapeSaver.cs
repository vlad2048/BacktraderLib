using BaseUtils;
using Feed.Trading212._sys.Structs;
using Feed.Trading212._sys.Utils;

namespace Feed.Trading212._sys;

static class ScrapeSaver
{
	public static bool IsScrapeNeeded(string secCompanyName, out QuarterSet quartersDone)
	{
		var symbolFile = Consts.Data.CompanyJsonFile(secCompanyName);
		quartersDone = new QuarterSet();
		if (!File.Exists(symbolFile))
		{
			return true;
		}
		else
		{
			var data = JsonUtils.Load<ScrapeData>(symbolFile);
			if (data.ErrorMessage == null && DateTime.Now - data.ScrapeTime <= Consts.ScrapeFreq)
				return false;
			quartersDone = data.Reports.ToDictionary(e => e.Key, e => e.Value.Keys.ToHashSet());
			return true;
		}
	}

	public static void Save(string secCompanyName, IScrapeResult scrapeResult)
	{
		if (scrapeResult is NoScrapeNeededScrapeResult) return;
		if (scrapeResult is not IResultScrapeResult result) throw new ArgumentException($"Invalid(1) ScrapeResult: {scrapeResult.GetType().FullName}");

		var reportsNext = result.Reports;
		var errorMessage = result.GetErrorMessage();
		var scrapeFile = Consts.Data.CompanyJsonFile(secCompanyName);
		var reportsPrev = File.Exists(scrapeFile) switch
		{
			true => JsonUtils.Load<ScrapeData>(scrapeFile).Reports,
			false => new Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>>(),
		};
		var reports = ReportMerger.Merge(reportsPrev, reportsNext);
		var scrapeData = new ScrapeData(
			secCompanyName,
			DateTime.Now,
			errorMessage,
			reports
		);
		scrapeData.Save(scrapeFile);
	}
}