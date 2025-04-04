using BaseUtils;
using Feed.Trading212._sys.Utils;
using FeedUtils;
using Microsoft.Playwright;

namespace Feed.Trading212;


enum LogCategory
{
	ScrapeLoop,
	ScrapeSymbol,
	Search,
	Request,
	Retry,
	RetryVerbose,
}



static class Consts
{
	public static readonly HashSet<LogCategory> EnabledLogCategories =
	[
		LogCategory.ScrapeLoop,
		//LogCategory.ScrapeSymbol,
		//LogCategory.Search,
		LogCategory.Request,
		//LogCategory.Retry,
		//LogCategory.RetryVerbose,
	];



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

	public const string ScriptProjFolder = @"D:\dev\big\BacktraderLib\Play\WebScript";
	public static readonly TimeSpan ScriptDebounceTime = TimeSpan.FromMilliseconds(500);


	
	public static readonly Quarter MinScrapeQuarter = Quarter.MinValue;
	//public static readonly Quarter MinScrapeQuarter = new(2024, QNum.Q1);

	public static readonly LocatorClickOptions Click_MoreFinancialsDelay = new()
	{
		Timeout = 10000,
	};

	public static readonly LocatorClickOptions Click_QuarterlyDelay = new()
	{
		Timeout = 500,
	};

	public static readonly RetryPolicy QuarterlyClickRetryPolicy = new(
		20,
		TimeSpan.FromSeconds(0.2),
		ex => ex is TimeoutException
	);
}