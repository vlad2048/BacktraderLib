using BaseUtils;
using Feed.Trading212._sys._1_Scraping;
using Feed.Trading212._sys._1_Scraping.Structs;
using Feed.Trading212._sys._1_Scraping.Utils;
using Feed.Trading212._sys._2_Processing;
using ScrapeUtils;

namespace Feed.Trading212;

public static class API
{
	public static string[] Companies => Consts.Processing.GetAllCompanies();

	public static Template Fields => Processor.LoadTemplate();

	public static CompanyReports Load(string companyName) => JsonUtils.Load<CompanyReports>(Consts.Processing.CompanyJsonFile(companyName));



	public static class Scraping
	{
		public static async Task Scrape(
			Web web,
			CompanyDef[] companies,
			CancellationToken cancelToken,
			ScrapeOpt? opt = null
		) =>
			await ScraperLogic.Scrape(
				web,
				companies,
				cancelToken,
				opt ?? new ScrapeOpt()
			);

		public static string[] GetCompanies() => Consts.Scraping.GetAllCompanies();

		public static CompanyScrapeState GetCompanyScrapeState(string companyName) =>
			StateFile.Map.TryGetValue(companyName, out var state) switch
			{
				true => state,
				false => throw new ArgumentException($"Company {companyName} not found"),
			};

		public static ScrapeData Load(string companyName) => JsonUtils.Load<ScrapeData>(Consts.Scraping.CompanyJsonFile(companyName));

		public static bool GetIsScrapeNeeded(string companyName, TimeSpan? refreshOldPeriod)
		{
			var state = StateFile.Map.TryGetValue(companyName, out var state_) switch
			{
				true => state_,
				false => null,
			};
			return StateFileHolderUtils.NeedsScraping(state, refreshOldPeriod);
		}

		internal static void Init_() => stateFile = JsonUtils.LoadOr(Consts.Scraping.StateFile, StateFile.Empty);

		static StateFile? stateFile;
		static StateFile StateFile => stateFile ?? throw new ArgumentException("You need to call FeedTrading212Setup.Init() first");
	}



	public static class Processing
	{
		public static void Run() => Processor.Process();
	}




	internal static void Init()
	{
		Scraping.Init_();
	}
}