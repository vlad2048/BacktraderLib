using FeedUtils;

namespace Feed.YahooOoples;

static class Consts
{
	static readonly string RootFolder = FileUtils.GetProjectRootFolder("Feed.YahooOoples");
	static readonly string DataFolder = Path.Combine(RootFolder, "Data").CreateFolderIFN();
	
	
	public static string GetSymbolFile(string symbol) => Path.Combine(DataFolder, symbol.GetPrefix(), $"{symbol}.json");
	
	public static readonly DateTime TimeStart = new(1900, 1, 1);
	public static readonly TimeSpan FetchDelay = TimeSpan.FromDays(2);



	static string GetPrefix(this string e) => e[..Math.Min(1, e.Length)];
}