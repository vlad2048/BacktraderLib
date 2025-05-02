using BaseUtils;

namespace Feed.Trading212._sys._1_Scraping.Utils;

static class QuarterSetExt
{
	public static bool ContainsReportQuarter(this QuarterSet set, ReportType reportType, Quarter quarter) =>
		set.TryGetValue(reportType, out var quarterMap) switch
		{
			true => quarterMap.Contains(quarter),
			false => false,
		};
}