using BaseUtils;
using Feed.YahooOoples._sys;
using FeedUtils;
using Frames;
using OoplesFinance.YahooFinanceAPI;
using OoplesFinance.YahooFinanceAPI.Models;

namespace Feed.YahooOoples;

public static class API
{
	static readonly YahooClient client = new();

	public static Frame<string, string, Bar> Fetch(string[] syms, bool adjClose = true) =>
		PriceBuilder.Build(
			syms.SelectA(e => (e, client.FetchPrice(e).Bars)),
			e => e.Date.Date,
			e => e.Open,
			e => e.High,
			e => e.Low,
			e => adjClose ? e.AdjClose : e.Close,
			e => e.Volume
		);

	public static OoplesPrice FetchFull(string sym) => client.FetchPrice(sym);


	public static bool SaveTodaySnap() => SnapTaker.SaveTodaySnap(client);
	public static Snap LoadSnap(DateOnly? date = null) => SnapTaker.LoadSnap(client, date);
}