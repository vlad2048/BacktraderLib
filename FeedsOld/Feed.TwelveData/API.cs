using BaseUtils;
using Feed.TwelveData._sys;
using FeedUtils;
using Frames;

namespace Feed.TwelveData;

public static class API
{
	public static void SetTwelveDataApiKey(string apiKey) => apiKey_ = apiKey;

	public static Frame<string, string, Bar> Fetch(string[] syms, Adjust adjust = Adjust.All) =>
		PriceBuilder.Build(
			syms.SelectA(e => (e, Query(e, null, null, Freq.Day, adjust))),
			e => e.Date,
			e => e.Open,
			e => e.High,
			e => e.Low,
			e => e.Close,
			e => e.Volume
		);

	public static TwelveDataBar[] Query(string symbol, DateTime? dateStart, DateTime? dateEnd, Freq freq, Adjust adjust) => QueryLogic.Query(symbol, dateStart, dateEnd, freq, adjust, ApiKey);


	static string? apiKey_ { get; set; }
	static string ApiKey => apiKey_ ?? throw new ArgumentException("Set TwelveData API key first using Feed.TwelveData.API.SetTwelveDataApiKey(string apiKey)");
}