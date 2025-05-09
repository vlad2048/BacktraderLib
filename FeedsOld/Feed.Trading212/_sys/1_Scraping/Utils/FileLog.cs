namespace Feed.Trading212._sys._1_Scraping.Utils;

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
