using BaseUtils;
using FeedUtils;
// ReSharper disable ClassNeverInstantiated.Local

namespace Feed.Symbology._sys;

static class Trading212SymbolFetcher
{
	public static Trading212SymbolData Fetch()
	{
		var instrs = Query<Instr>("instruments");
		var exchgs = Query<Exch>("exchanges");
		var map = (
			from exch in exchgs
			from sched in exch.workingSchedules
			select (exch.name, sched.id)
		).ToDictionary(t => t.id, t => t.name);

		var symbols = instrs.SelectA(e => new Trading212Symbol(
			e.ticker,
			e.type,
			map[e.workingScheduleId],
			e.workingScheduleId,
			e.isin,
			e.currencyCode,
			e.name,
			e.shortName,
			e.minTradeQuantity,
			e.maxOpenQuantity,
			e.addedOn
		));

		var schedules = (
			from exch in exchgs
			from sched in exch.workingSchedules
			select (sched.id, sched)
		).ToDictionary(
			t => t.id,
			t => t.sched.timeEvents.SelectA(e => new Trading212ScheduleEvent(
				e.date,
				e.type switch
				{
					"OPEN" => Trading212ScheduleEventType.Open,
					"CLOSE" => Trading212ScheduleEventType.Close,
					"PRE_MARKET_OPEN" => Trading212ScheduleEventType.PreMarketOpen,
					"AFTER_HOURS_OPEN" => Trading212ScheduleEventType.AfterHoursOpen,
					"OVERNIGHT_OPEN" => Trading212ScheduleEventType.OvernightOpen,
					"AFTER_HOURS_CLOSE" => Trading212ScheduleEventType.AfterHoursClose,
					_ => throw new ArgumentException($"Invalid Trading212ScheduleEventType: {e.type}"),
				}
			))
		);

		return new Trading212SymbolData(
			symbols,
			schedules
		);
	}


	record Evt(DateTime date, string type);

	record Sched(int id, Evt[] timeEvents);

	record Exch(string name, Sched[] workingSchedules);

	sealed record Instr(
		string ticker,
		string type,
		int workingScheduleId,
		string isin,
		string currencyCode,
		string name,
		string shortName,
		decimal minTradeQuantity,
		decimal maxOpenQuantity,
		DateTime addedOn
	);


	static T[] Query<T>(string path)
	{
		var response = Client.GetAsync($"{Url}/{path}").Result;
		if (!response.IsSuccessStatusCode) throw new ArgumentException($"Error querying {Url}. {response.StatusCode}");
		var text = response.Content.ReadAsStringAsync().Result;
		return JsonUtilsGeneric.Deser<T[]>(text);
	}

	
	static readonly Lazy<HttpClient> client = new(() => new HttpClient
	{
		DefaultRequestHeaders =
		{
			{ "Authorization", API.ApiKeys.Trading212 },
		},
	});
	static HttpClient Client => client.Value;

	const string Url = "https://live.trading212.com/api/v0/equity/metadata";
}