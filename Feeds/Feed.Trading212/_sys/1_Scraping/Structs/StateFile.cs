using BaseUtils;
using Feed.Trading212._sys._1_Scraping.Utils;

namespace Feed.Trading212._sys._1_Scraping.Structs;

sealed record StateFile(
	Dictionary<string, CompanyScrapeState> Map
)
{
	public static StateFile Empty => new(new Dictionary<string, CompanyScrapeState>());
}


sealed class StateFileHolder(bool disableSaving)
{
	readonly StateFile state = JsonUtils.LoadOr(Consts.Scraping.StateFile, StateFile.Empty);

	public CompanyScrapeState? GetState(string companyName) =>
		state.Map.TryGetValue(companyName, out var state_) switch
		{
			true => state_,
			false => null,
		};

	public void SetState(string companyName, CompanyScrapeStatus status, string? error)
	{
		state.Map[companyName] = new CompanyScrapeState(status, DateTime.Now, error);
		if (!disableSaving)
			JsonUtils.Save(state, Consts.Scraping.StateFile);
	}
}

static class StateFileHolderUtils
{
	public static bool NeedsScraping(this StateFileHolder stateFile, CompanyDef company, TimeSpan? refreshOldPeriod)
	{
		var state = stateFile.GetState(company.Name);
		return NeedsScraping(state, refreshOldPeriod);
	}

	public static bool NeedsScraping(CompanyScrapeState? state, TimeSpan? refreshOldPeriod)
	{
		if (state is null) return true;
		var isTooOld = refreshOldPeriod.HasValue && DateTime.Now - state.LastScrapeTime >= refreshOldPeriod.Value;
		return state.Status switch
		{
			CompanyScrapeStatus.Success => isTooOld,
			CompanyScrapeStatus.InProgress => true,
			CompanyScrapeStatus.Error => false,
			_ => throw new ArgumentException($"Unknown CompanyScrapeStatus: {state.Status}"),
		};
	}
}