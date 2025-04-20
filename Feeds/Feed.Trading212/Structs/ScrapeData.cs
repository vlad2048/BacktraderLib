using BaseUtils;

namespace Feed.Trading212;


public record ScrapeData(
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
);

