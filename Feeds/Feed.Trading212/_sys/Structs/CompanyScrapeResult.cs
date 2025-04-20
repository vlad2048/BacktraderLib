using BaseUtils;

namespace Feed.Trading212._sys.Structs;

sealed record CompanyScrapeResult(
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports,
	ScrapeError? Error
);

/*interface IScrapeResult;
interface IResultScrapeResult : IScrapeResult
{
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports { get; }
}

sealed record NoScrapeNeededScrapeResult : IScrapeResult;

sealed record SuccessScrapeResult(
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
) : IResultScrapeResult;

sealed record ErrorScrapeResult(
	Exception Ex,
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
) : IResultScrapeResult;

sealed record RateLimitScrapeResult(
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
) : IResultScrapeResult;

sealed record NoInternetScrapeResult(
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
) : IResultScrapeResult;


static class ScrapeResult
{
	public static readonly IScrapeResult NoScrapeNeeded = new NoScrapeNeededScrapeResult();
	public static IScrapeResult Success(Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> reports) => new SuccessScrapeResult(reports);
	public static IScrapeResult Error(Exception ex, Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> partialReports) => new ErrorScrapeResult(ex, partialReports);
	public static IScrapeResult RateLimit(Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> reports) => new RateLimitScrapeResult(reports);
	public static IScrapeResult NoInternet(Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> reports) => new NoInternetScrapeResult(reports);
}


static class ScrapeResultUtils
{
	public static string? GetErrorMessage(this IResultScrapeResult result) =>
		result switch
		{
			SuccessScrapeResult => null,
			ErrorScrapeResult e => e.Ex.Message,
			RateLimitScrapeResult => "RateLimit",
			NoInternetScrapeResult => "NoInternet",
			_ => throw new ArgumentException($"Invalid(2) ScrapeResult: {result.GetType().FullName}"),
		};

	public static ScrapeStatus GetScrapeStatus(this IScrapeResult result) =>
		result switch
		{
			NoScrapeNeededScrapeResult => ScrapeStatus.None,
			SuccessScrapeResult => ScrapeStatus.None,
			ErrorScrapeResult { Ex: var ex } => throw ex,
			RateLimitScrapeResult => ScrapeStatus.RateLimit,
			NoInternetScrapeResult => ScrapeStatus.NoInternet,
			_ => throw new ArgumentException($"Invalid(3) ScrapeResult: {result.GetType().FullName}"),
		};
}*/


static class ReportMerger
{
	public static Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Merge(Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> mapPrev, Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> mapNext)
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
