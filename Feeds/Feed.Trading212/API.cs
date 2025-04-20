using BaseUtils;
using Feed.Trading212._sys;
using Feed.Trading212._sys.Structs;
using Feed.Trading212._sys.Utils;
using ScrapeUtils;

namespace Feed.Trading212;


public sealed class ScrapeOpt
{
	public bool DryRun { get; init; }
	public bool DisableSaving { get; init; }
	public TimeSpan? RefreshOldPeriod { get; init; }
	public string? InvalidRequestSaveFile { get; init; }
}


public static class API
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

	public static ScrapeData Load(string companyName) => JsonUtils.Load<ScrapeData>(Consts.Data.CompanyJsonFile(companyName));


	static readonly Lazy<StateFile> stateFile = new(() => JsonUtils.LoadOr(Consts.StateFile, StateFile.Empty));

	public static bool GetIsScrapeNeeded(string companyName, TimeSpan? refreshOldPeriod)
	{
		var state = stateFile.Value.Map.TryGetValue(companyName, out var state_) switch
		{
			true => state_,
			false => null,
		};
		return StateFileHolderUtils.NeedsScraping(state, refreshOldPeriod);
	}
}
