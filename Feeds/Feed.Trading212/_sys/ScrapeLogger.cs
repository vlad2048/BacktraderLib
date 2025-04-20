using BaseUtils;
using Feed.Trading212._sys.Utils;
using LINQPad;
using ScrapeUtils;

namespace Feed.Trading212._sys;

sealed class ScrapeLogger
{
	static readonly ReportType[] ReportTypes = Enum.GetValues<ReportType>();

	readonly DumpContainer dcProgress;
	readonly DumpContainer dcStats;
	readonly DumpContainer dcErrors;

	Progress progress = Progress.Empty;

	public ScrapeLogger(DumpContainer dcLog, ScrapeOpt opt, int companies, int companiesTodo)
	{
		LogStart(dcLog, opt, companies, companiesTodo);
		dcProgress = dcLog.AddNewDC("Progress");
		dcStats = dcLog.AddNewDC("RetryStats");
		dcErrors = dcLog.AddNewDC("Errors");
	}

	public void ProgressCompany(string company, int idx, int cnt)
	{
		progress = progress with { Company = (company, idx, cnt), Report = null, Quarter = null };
		dcProgress.UpdateContent(progress);
	}
	public void ProgressReport(ReportType report)
	{
		progress = progress with { Report = (report, ReportTypes.IdxOf(report), ReportTypes.Length), Quarter = null };
		dcProgress.UpdateContent(progress);
	}
	public void ProgressQuarter(Quarter quarter, int idx, int cnt)
	{
		progress = progress with { Quarter = (quarter, idx, cnt) };
		dcProgress.UpdateContent(progress);
	}

	public void LogStats(FullStatsKeeper stats) => dcStats.UpdateContent(stats);
	
	public void LogImpossibleException(Exception ex) => dcErrors.AppendContent(ex);

	public void LogCancel() => dcErrors.AppendContent(Util.RawHtml("<h1 style='color:#72b4ef'>Cancel</h1>"));


	static void LogStart(DumpContainer dc, ScrapeOpt opt, int companies, int companiesTodo)
	{
		dc.LogH2("Trading212 Scraping");
		dc.LogH3("Options");
		dc.Log($"    RefreshOldPeriod      : {opt.RefreshOldPeriod}");
		dc.Log($"    DryRun                : {opt.DryRun}");
		dc.Log($"    DisableSaving         : {opt.DisableSaving}");
		dc.Log($"    InvalidRequestSaveFile: {opt.InvalidRequestSaveFile}");
		dc.LogH3("Companies");
		dc.Log($"    companies    : {companies}");
		dc.Log($"    companiesTodo: {companiesTodo}");
		dc.Log("");
	}



	sealed record Progress(
		(string, int, int)? Company,
		(ReportType, int, int)? Report,
		(Quarter, int, int)? Quarter
	)
	{
		public static readonly Progress Empty = new(null, null, null);

		public object ToDump() => Util.Pivot(new
		{
			Company = Company.Fmt(),
			Report = Report.Fmt(),
			Quarter = Quarter.Fmt(),
		});
	}
}



static class ScrapeLoggerUtils
{
	public static string Fmt<T>(this (T, int, int)? t) => t switch
	{
		null => "_",
		not null => t.Value.Item2.perc(t.Value.Item3) + $"  {t.Value.Item1}",
	};
}
