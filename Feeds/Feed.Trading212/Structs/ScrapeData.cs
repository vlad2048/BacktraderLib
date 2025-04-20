using BaseUtils;

namespace Feed.Trading212;


public record ScrapeData(
	string Company,
	DateTime ScrapeTime,
	string? ErrorMessage,
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
);

