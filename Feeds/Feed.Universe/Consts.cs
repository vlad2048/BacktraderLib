using FeedUtils;

namespace Feed.Universe;

static class Consts
{
	public static readonly string RootFolder = FileUtils.GetProjectRootFolder("Feed.Universe");


	public static class StockAnalysis
	{
		static readonly string Folder = Path.Combine(RootFolder, "stockanalysis").CreateFolderIFN();

		public static FetchLimiter FetchLimiter(IUniverse universe) => new(
			Path.Combine(Folder, $"fetchlimiter-{universe}.json"),
			TimeSpan.FromDays(7)
		);

		public static string DataFile(IUniverse universe) => Path.Combine(Folder, $"data-{universe}.json");
	}


	public static class TwelveDataSymbols
	{
		static readonly string Folder = Path.Combine(RootFolder, "twelvedata").CreateFolderIFN();

		public static readonly FetchLimiter FetchLimiter = new(
			Path.Combine(Folder, "fetchlimiter.json"),
			TimeSpan.FromDays(7)
		);

		public static readonly string SymbolsFile = Path.Combine(Folder, "symbols.json");
	}
}