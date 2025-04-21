using BaseUtils;
using Feed.Trading212._sys.Structs;
using System.Text;

namespace Feed.Trading212._sys.Utils;

sealed class FileLog : IDisposable
{
	readonly FileStream fs;
	readonly StreamWriter sw;

	public void Dispose()
	{
		sw.Dispose();
		fs.Dispose();
	}

	public FileLog(string file)
	{
		fs = new FileStream(file, FileMode.Create);
		sw = new StreamWriter(fs)
		{
			AutoFlush = true,
		};
		Log("new FileLog()");
	}

	public void Log(string msg) => sw.WriteLine($"[{Timestamp}] {msg}");

	static string Timestamp => $"{DateTime.Now:HH:mm:ss.ffffff}";
}



static class FileLogUtils
{
	public static void LogStart(this FileLog fileLog, int companiesTodo) => fileLog.Log($"Start({companiesTodo})");

	public static void LogProgress(this FileLog fileLog, string company, int idx, int cnt, int tryIdx)
	{
		var sb = new StringBuilder();
		sb.Append($"[{idx.perc(cnt)}]  '{company}'");
		if (tryIdx > 0)
			sb.Append($"   Retry {tryIdx}/{Consts.MaxCompanyScrapeRetryCount - 1}");
		var str = sb.ToString();
		fileLog.Log(str);
	}

	public static void LogImpossibleException(this FileLog fileLog, Exception ex) => fileLog.Log($"Impossible exception in outer loop (isCancel:{ex.IsCancel()}): {ex}");

	public static void LogFinished(this FileLog fileLog) => fileLog.Log("Finished");

	public static void LogErrorResponse(this FileLog fileLog, IErrorResponse errorResponse)
	{
		switch (errorResponse)
		{
			case NoneErrorResponse:
				break;

			case StopImmediatlyErrorResponse:
				fileLog.Log("Cancel");
				break;

			case FlagAsErrorErrorResponse { Error: var error, Reason: var reason }:
				fileLog.Log($"Error => FlagAsError(reason={reason})    ({error})");
				break;

			case WaitAndRetryErrorResponse { Error: var error, DelaySeconds: var delay }:
				fileLog.Log($"Error => WaitAndRetry(delay={delay}sec)    ({error})");
				break;
		}
	}
}