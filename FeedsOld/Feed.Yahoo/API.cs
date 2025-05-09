using BaseUtils;
using Feed.Yahoo._sys;
using FeedUtils;
using Frames;

namespace Feed.Yahoo;

public static class API
{
	public static Frame<string, string, Bar> Fetch(string[] syms) =>
		PriceBuilder.Build(
			syms.SelectA(e => (e, Query(e, null, null, Freq.Day, Adjust.All))),
			e => e.Date,
			e => e.Open,
			e => e.High,
			e => e.Low,
			e => e.Close,
			e => e.Volume
		);

	public static YahooBar[] Query(string symbol, DateTime? dateStart, DateTime? dateEnd, Freq freq, Adjust adjust) => QueryLogic.Query(symbol, dateStart, dateEnd, freq, adjust);
}