using BaseUtils;
using LINQPad;
using ScrapeUtils;

namespace Feed.Trading212._sys;

sealed class ScrapeLogger
{
	static readonly ReportType[] ReportTypes = Enum.GetValues<ReportType>();

	readonly DumpContainer logDC;
	readonly DumpContainer ReportProgressDC;
	readonly DumpContainer QuarterProgressDC;
	readonly DumpContainer StatsDC;

	public ScrapeLogger(DumpContainer logDC)
	{
		this.logDC = logDC;
		logDC.AppendContent(ReportProgressDC = new DumpContainer());
		logDC.AppendContent(QuarterProgressDC = new DumpContainer());
		logDC.AppendContent(StatsDC = new DumpContainer());
	}

	public void LogReportProgress(ReportType reportType) => ReportProgressDC.UpdateContent($"Report {ReportTypes.IdxOf(reportType).perc(ReportTypes.Length)}");
	public void LogQuarterProgress(int idx, int cnt) => QuarterProgressDC.UpdateContent($"Quarter {idx.perc(cnt)}");

	public void LogStats(FullStats fullStats) => StatsDC.UpdateContent(fullStats);

	public void LogFinished() => logDC.AppendContent("Finished");
}