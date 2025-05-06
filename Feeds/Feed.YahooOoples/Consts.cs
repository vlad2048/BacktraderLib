using BaseUtils;
using FeedUtils;

namespace Feed.YahooOoples;

static class Consts
{
	static readonly string RootFolder = FileUtils.GetProjectRootFolder("Feed.YahooOoples");

	/*
	public const FetchStrat FetchStrat = _sys.Structs.FetchStrat.None;

	public static class Data
	{
		static readonly string Folder = Path.Combine(RootFolder, "Data").CreateFolderIFN();
		public static string GetSymbolFile(string symbol) => Path.Combine(Folder, symbol.GetPrefix(), $"{symbol}.json");
	}
	*/

	public static class Snap
	{
		static readonly string Folder = Path.Combine(RootFolder, "Snap").CreateFolderIFN();
		public static DateOnly[] GetAllDates() => Directory.GetFiles(Folder, "*.json").SelectA(e => DateOnly.Parse(Path.GetFileNameWithoutExtension(e)));
		public static string GetDateSnapFile(DateOnly date) => Path.Combine(Folder, $"{date:yyyy-MM-dd}.json");
	}



	public static readonly DateTime TimeStart = new(1900, 1, 1);
	//public static readonly TimeSpan FetchDelay = TimeSpan.FromDays(2);



	//static string GetPrefix(this string e) => e[..Math.Min(1, e.Length)];
}