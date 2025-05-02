using BaseUtils;

namespace Feed.Trading212;


public sealed record ScrapeData(
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
);