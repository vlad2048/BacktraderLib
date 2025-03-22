using BaseUtils;
using Feed.TwelveData._sys;
using FeedUtils;
using Frames;

namespace Feed.TwelveData;

public static class API
{
	public static Frame<string, string, Bar> Fetch(string[] syms, string apiKey) =>
		PriceBuilder.Build(
			syms.SelectA(e => (e, Query(e, null, null, Freq.Day, Adjust.All, apiKey))),
			e => e.Date,
			e => e.Open,
			e => e.High,
			e => e.Low,
			e => e.Close,
			e => e.Volume
		);

	public static TwelveDataBar[] Query(string symbol, DateTime? dateStart, DateTime? dateEnd, Freq freq, Adjust adjust, string apiKey) => QueryLogic.Query(symbol, dateStart, dateEnd, freq, adjust, apiKey);
}