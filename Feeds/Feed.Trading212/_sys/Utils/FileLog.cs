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

	public static void LogImpossibleException(this FileLog fileLog, Exception ex) => fileLog.Log($"Impossible exception in outer loop: {ex}");

	public static void LogCancel(this FileLog fileLog) => fileLog.Log("Cancel");
	public static void LogFinished(this FileLog fileLog) => fileLog.Log("Finished");

	public static void LogError(this FileLog fileLog, ScrapeError error, int delay) => fileLog.Log($"Error: {error.Type}    wait {delay}sec    ({error.Message})");
	public static void LogReachedMaxTries(this FileLog fileLog) => fileLog.Log("Error: Reached Max Tries");
}