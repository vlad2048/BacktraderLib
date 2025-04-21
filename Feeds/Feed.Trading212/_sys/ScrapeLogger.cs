using BaseUtils;
using Feed.Trading212._sys.Structs;
using Feed.Trading212._sys.Utils;
using LINQPad;
using ScrapeUtils;

namespace Feed.Trading212._sys;

sealed class ScrapeLogger : IDisposable
{
	static readonly ReportType[] ReportTypes = Enum.GetValues<ReportType>();

	public void Dispose() => fileLog.Dispose();

	readonly FileLog fileLog;
	readonly DumpContainer dcProgress;
	readonly DumpContainer dcStats;
	readonly DumpContainer dcErrors;

	Progress progress = Progress.Empty;

	public ScrapeLogger(DumpContainer dcLog, ScrapeOpt opt, int companies, int companiesTodo)
	{
		fileLog = new FileLog(Consts.Logs.LogFile);
		LogStart(dcLog, opt, companies, companiesTodo);
		dcProgress = dcLog.AddNewDC("Progress");
		dcStats = dcLog.AddNewDC("RetryStats");
		dcErrors = dcLog.AddNewDC("Errors");
	}

	public void ProgressCompany(string company, int idx, int cnt, int tryIdx)
	{
		// FileLog
		fileLog.Log($"[{idx.perc(cnt)}]  '{company}'" + (tryIdx > 0 ? $"   Retry {tryIdx}/{Consts.MaxCompanyScrapeRetryCount - 1}" : ""));
		// DumpContainer
		progress = progress with { Company = (company, idx, cnt), Report = null, Quarter = null };
		dcProgress.UpdateContent(progress);
		dcErrors.ClearContent();
	}
	public void ProgressReport(ReportType report)
	{
		// FileLog
		// DumpContainer
		progress = progress with { Report = (report, ReportTypes.IdxOf(report), ReportTypes.Length), Quarter = null };
		dcProgress.UpdateContent(progress);
	}
	public void ProgressQuarter(Quarter quarter, int idx, int cnt)
	{
		// FileLog
		// DumpContainer
		progress = progress with { Quarter = (quarter, idx, cnt) };
		dcProgress.UpdateContent(progress);
	}

	public void LogStats(FullStatsKeeper stats)
	{
		// FileLog
		// DumpContainer
		dcStats.UpdateContent(stats);
	}

	public void LogImpossibleException(Exception ex)
	{
		// FileLog
		fileLog.Log($"Impossible exception in outer loop (isCancel:{ex.IsCancel()}): {ex}");
		// DumpContainer
		dcErrors.AppendContent(ex);
	}

	public void LogFinished()
	{
		// FileLog
		fileLog.Log("Finished");
		// DumpContainer
	}

	public void LogErrorResponse(IErrorResponse errorResponse)
	{
		var str = errorResponse.GetLogMessage();
		var ex = errorResponse.GetLogException();
		if (str != null)
		{
			// FileLog
			fileLog.Log(str);
			// DumpContainer
			dcErrors.AppendContent(Util.RawHtml($"<h1 style='color:#72b4ef'>{str}</h1>"));
		}
		if (ex != null)
		{
			// ErrorFile
			ErrorFileUtils.Log(ex);
		}
	}

	

	void LogStart(DumpContainer dcLog, ScrapeOpt opt, int companies, int companiesTodo)
	{
		fileLog.Log($"Start({companiesTodo})");

		dcLog.LogH2("Trading212 Scraping");
		dcLog.LogH3("Options");
		dcLog.Log($"    RefreshOldPeriod      : {opt.RefreshOldPeriod}");
		dcLog.Log($"    DryRun                : {opt.DryRun}");
		dcLog.Log($"    DisableSaving         : {opt.DisableSaving}");
		dcLog.LogH3("Companies");
		dcLog.Log($"    all : {companies}");
		dcLog.Log($"    todo: {companiesTodo}");
		dcLog.Log("");
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



file static class ScrapeLoggerUtils
{
	public static string Fmt<T>(this (T, int, int)? t) => t switch
	{
		null => "_",
		not null => t.Value.Item2.perc(t.Value.Item3) + $"  {t.Value.Item1}",
	};
}
