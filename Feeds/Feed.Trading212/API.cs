using Feed.Trading212._sys;
using Feed.Trading212._sys.Structs;
using Feed.Trading212._sys.Utils;
using ScrapeUtils;

namespace Feed.Trading212;


public sealed class ScrapeOpt
{
	public bool DisableSaving { get; init; }
	public string? InvalidRequestSaveFile { get; init; }
}

public enum ScrapeStatus
{
	None,
	RateLimit,
	NoInternet,
}


public static class API
{
	public static string[] GetAllCompanies() => Consts.Data.GetAllCompanies();

	public static bool IsScrapeNeeded(string secCompanyName) => ScrapeSaver.IsScrapeNeeded(secCompanyName, out _);

	public static async Task<ScrapeStatus> Scrape(Web web, CompanySearchInfo companySearchInfo, ScrapeOpt? opt)
	{
		opt ??= new ScrapeOpt();

		var result = await ScraperLogic.Scrape(web, companySearchInfo, opt);

		if (!opt.DisableSaving)
			ScrapeSaver.Save(companySearchInfo.SecCompanyName, result);

		return result.GetScrapeStatus();
	}

	public static ScrapeData Load(string secCompany) => JsonUtils.Load<ScrapeData>(Consts.Data.CompanyJsonFile(secCompany));
}
