using Feed.TwelveData._sys.Errors;
using Feed.TwelveData._sys.Structs;
using FeedUtils;

namespace Feed.TwelveData._sys;

static class QueryLogic
{
	static HttpClient? client;
	static HttpClient Client => client ?? throw new ArgumentException("HttpClient not initialized. This shouldn't be possible");


	public static TwelveDataBar[] Query(string symbol, DateTime? tMin, DateTime? tMax, Freq freq, Adjust adjust, string apiKey)
	{
		client ??= new HttpClient
		{
			DefaultRequestHeaders =
			{
				{ "Authorization", $"apikey {apiKey}" },
			},
		};

		var res = QueryInternalSingle(symbol, freq, adjust, tMin, tMax, Consts.MaxOutputSize);
		var list = new List<TimeSeriesResponse> { res };

		while (res.Values.Length == Consts.MaxOutputSize)
		{
			res = QueryInternalSingle(symbol, freq, adjust, tMin, res.Values[0].Datetime, Consts.MaxOutputSize);
			list.Add(res);
		}

		return TimeSeriesResponseUtils.Combine(list)
			.Validate()
			.ExtractBars();
	}




	static TimeSeriesResponse QueryInternalSingle(
		string symbol,
		Freq freq,
		Adjust adjust,
		DateTime? tMin,
		DateTime? tMax,
		int? outputSize
	)
	{
		var url = UrlBuilder.Build($"{Consts.QueryUrl}/time_series", e => e
			.Add("symbol", symbol)
			.Add("interval", freq.Ser().RemoveDoubleQuotes())
			.Add("adjust", adjust.Ser().RemoveDoubleQuotes())
			.Add("order", "asc")
			.Add("dp", "11")
			.AddIf("outputsize", outputSize)
			.AddIf("start_date", tMin, FmtDate)
			.AddIf("end_date", tMax, FmtDate)
		);
		var req = new HttpRequestMessage(HttpMethod.Get, url);
		HttpResponseMessage response;
		try
		{
			response = Client.Send(req);
		}
		catch (Exception ex)
		{
			throw TwelveDataException.MakeHttpRequestException(symbol, ex);
		}

		var responseStr = response.Content.ReadAsStringAsync().Result;

		if (!response.IsSuccessStatusCode)
		{
			var statusResponse = responseStr.Deser<StatusResponse>();
			if (statusResponse.Status != "ok")
			{
				var apiError = responseStr.Deser<TwelveDataError>();
				throw TwelveDataException.MakeApiError(symbol, apiError);
			}
		}

		return responseStr
			.Deser<TimeSeriesResponse>()
			.Validate();
	}

	// ReSharper disable once ClassNeverInstantiated.Local
	sealed record StatusResponse(string Status);

	static string FmtDate(DateTime e) => $"{e:yyyy-MM-dd}";
}