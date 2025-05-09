using BaseUtils;
using Feed.Trading212._sys._1_Scraping.Structs;
using FeedUtils;

namespace Feed.Trading212;


static class Consts
{
	static readonly string RootFolder = FileUtils.GetProjectRootFolder("Feed.Trading212");



	public static class Scraping
	{
		public static readonly string StateFile = Path.Combine(RootFolder, "scrape_state.json");
		static readonly string Folder = Path.Combine(RootFolder, "1_scraping").CreateFolderIFN();
		public static string[] GetAllCompanies() => Directory.GetFiles(Folder, "*.json").FromAllFilesSafe();
		public static string CompanyJsonFile(string company) => Path.Combine(Folder, $"{company.ToFileSafe()}.json");
	}



	public static class Processing
	{
		public static readonly string TemplateFile = Path.Combine(RootFolder, "processing_template.json");
		static readonly string Folder = Path.Combine(RootFolder, "2_processing").CreateFolderIFN();
		public static string[] GetAllCompanies() => Directory.GetFiles(Folder, "*.json").FromAllFilesSafe();
		public static string CompanyJsonFile(string company) => Path.Combine(Folder, $"{company.ToFileSafe()}.json");
	}



	public static class Logs
	{
		static readonly string Folder = Path.Combine(RootFolder, "logs").CreateFolderIFN();
		public static readonly string LogFile = Path.Combine(Folder, "log.txt");
		public static readonly string InvalidRequestFile = Path.Combine(Folder, "invalid-request.html");
		public static readonly string ErrorFile = Path.Combine(Folder, "errors.json");
	}


	public const string MainUrl = "https://app.trading212.com/";


	public static readonly Quarter MinScrapeQuarter = Quarter.MinValue;

	public static readonly bool[] QuarterLoops = [true, true, false];


	public const int MaxCompanyScrapeRetryCount = 5;

	public static IErrorResponse GetErrorResponse(ScrapeError? error, bool isLastAttempt) =>
		(error?.Type, isLastAttempt) switch
		{
			(null, _) => ErrorResponse.None,
			(ScrapeErrorType.Cancelled, _) => ErrorResponse.StopImmediatly,
			(ScrapeErrorType.CompanyNotFound, _) => ErrorResponse.FlagAsError(error, FlagAsErrorReason.CompanyNotFound),
			(_, false) => ErrorResponse.WaitAndRetry(error, 60),
			(_, true) => ErrorResponse.FlagAsError(error, FlagAsErrorReason.ReachedMaxTries),
		};
}
