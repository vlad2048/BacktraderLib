using BaseUtils;

namespace Feed.Trading212;

public sealed record CompanyReports(Dictionary<ReportType, SortedDictionary<Quarter, FieldVal[]>> Reports);