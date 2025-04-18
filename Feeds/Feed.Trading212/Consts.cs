using BaseUtils;
using FeedUtils;

namespace Feed.Trading212;


static class Consts
{
	public static readonly string RootFolder = FileUtils.GetProjectRootFolder("Feed.Trading212");


	public static class Data
	{
		static readonly string Folder = Path.Combine(RootFolder, "data").CreateFolderIFN();
		public static string[] GetAllCompanies() => Directory.GetFiles(Folder, "*.json").FromAllFilesSafe();
		public static string CompanyJsonFile(string company) => Path.Combine(Folder, $"{company.ToFileSafe()}.json");
		public static string[] GetAllCompanyJsonFiles() => GetAllCompanies().SelectA(CompanyJsonFile);
	}


	public const string MainUrl = "https://app.trading212.com/";
	public static readonly TimeSpan ScrapeFreq = TimeSpan.FromDays(30);
	public static readonly Quarter MinScrapeQuarter = Quarter.MinValue;
	public static readonly bool[] QuarterLoops = [true, true, false];
}