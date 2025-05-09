using Feed.Yahoo._sys.Errors;
using System.Text.Json;
using Feed.Yahoo._sys.Utils;
using FeedUtils;

namespace Feed.Yahoo._sys;

static class QueryLogic
{
	static readonly HttpClient client = new();


	public static YahooBar[] Query(string symbol, DateTime? dateStart, DateTime? dateEnd, Freq freq, Adjust adjust)
	{
		if (YahooErrorClassifier.IsIgnoredSymbol(symbol))
			throw YahooException.MakeSymbolIsIgnoredException(symbol);

		var tMin = dateStart ?? FindFirstTradeDate(symbol);
		var tMax = dateEnd ?? DateTime.Now.Date;
		var res = QueryInternalSingle(symbol, tMin, tMax, freq);
		return res.ExtractBars(adjust);
	}




	static DateTime FindFirstTradeDate(string symbol)
	{
		var tMax = DateTime.Now.Date;
		var tMin = tMax - TimeSpan.FromDays(16);
		var res = QueryInternalSingle(symbol, tMin, tMax, Freq.Day);
		return res.Meta.FirstTradeDate ?? throw new ArgumentException("Impossible: FirstTradeDate is null");
	}



	static YahooResult QueryInternalSingle(string symbol, DateTime tMin, DateTime tMax, Freq freq)
	{
		try
		{
			var url = UrlBuilder.Build($"{Consts.QueryUrl}/{symbol}", e => e
				.Add("period1", tMin.ToUnixTime())
				.Add("period2", tMax.ToUnixTime())
				.Add("interval", freq.Ser().RemoveDoubleQuotes())
			);
			var req = new HttpRequestMessage(HttpMethod.Get, url);
			req.Headers.Add("User-Agent", Consts.UserAgent);
			HttpResponseMessage response;
			try
			{
				response = client.Send(req);
			}
			catch (Exception ex)
			{
				throw YahooException.MakeHttpRequestException(symbol, ex);
			}

			var responseStringRaw = response.Content.ReadAsStringAsync().Result;
			var responseString = responseStringRaw.Beautify();

			if (!response.IsSuccessStatusCode)
			{
				var error = DeserializeResponseError(responseString);
				throw YahooException.MakeHttpResponseException(symbol, response.StatusCode, error);
			}

			var result = DeserializeResponseString(responseString, symbol);
			return result;

		}
		catch (YahooException ex) when (ex.DoesErrorNeedResponseFileWritten())
		{
			var errorFilename = ex.ErrorFilename ?? throw new ArgumentException("Impossible");
			var responseString = ex.ResponseString ?? throw new ArgumentException("Impossible");
			File.WriteAllText(errorFilename, responseString);
			ex.ResponseString = null;
			throw;
		}
	}




	static YahooError DeserializeResponseError(string responseString)
	{
		try
		{
			var res = JsonUtils.Deser<YahooResponse>(responseString);
			// ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
			return (res?.Chart?.Error is not null) switch
			{
				true => res.Chart.Error,
				false => new YahooError("Error not found in response", "n/a"),
			};
		}
		catch (Exception ex)
		{
			return new YahooError("Failed to read error from response", $"{ex}");
		}
	}





	static YahooResult DeserializeResponseString(string responseString, string symbol)
	{
		YahooResponse response;
		try
		{
			response = JsonUtils.Deser<YahooResponse>(responseString);
		}
		catch (JsonException ex)
		{
			throw YahooException.MakeJsonException(symbol, ex, responseString);
		}

		YahooException Err(string msg) => YahooException.MakeInvalidResponseException(symbol, responseString, msg);


		if (response.Chart == null!) throw Err("Response.Chart == null");
		if (response.Chart.Error != null) throw Err($"Response.Chart.Error != null ({response.Chart.Error})");
		if (response.Chart.Result.Length != 1) throw Err($"Response.Chart.Result.Length != 1 ({response.Chart.Result.Length != 1})");

		var result = response.Chart.Result[0];

		if (result.Meta == null!) throw Err("Result.Meta == null");
		if (result.Meta.FirstTradeDate == null) throw Err(YahooErrorClassifier.Msg_ResultMetaFirstTradeDate_is_null);
		if (result.Timestamp == null) throw Err(YahooErrorClassifier.Msg_ResultTimestamp_is_null);
		if (result.Indicators == null!) throw Err("Result.Indicators == null");
		if (result.Indicators.Quote == null!) throw Err("Result.Indicators.Quote == null");
		if (result.Indicators.Quote.Length != 1) throw Err($"Result.Indicators.Quote.Length != 1 ({result.Indicators.Quote.Length})");
		//if (result.Indicators.Adjclose == null!) throw Err("Result.Indicators.Adjclose == null");
		//if (result.Indicators.Adjclose.Length != 1) throw Err("result.Indicators.Adjclose.Length != 1");

		var quote = result.Indicators.Quote[0];
		//var adjClose = result.Indicators.Adjclose[0];

		if (quote.Volume == null!) throw Err("Quote.Volume == null");
		if (quote.High == null!) throw Err("Quote.High == null");
		if (quote.Open == null!) throw Err("Quote.Open == null");
		if (quote.Close == null!) throw Err("Quote.Close == null");
		if (quote.Low == null!) throw Err("Quote.Low == null");
		//if (adjClose.Adjclose == null!) throw Err("AdjClose.Adjclose == null");

		var cnt = result.Timestamp.Length;

		if (result.Timestamp.Length == 0) throw Err("Result.Timestamp.Length == 0");
		if (quote.Volume.Length != cnt) throw Err($"Quote.Volume.Length != Result.Timestamp.Length ({quote.Volume.Length} != {cnt})");
		if (quote.High.Length != cnt) throw Err($"Quote.High.Length != Result.Timestamp.Length ({quote.High.Length} != {cnt})");
		if (quote.Open.Length != cnt) throw Err($"Quote.Open.Length != Result.Timestamp.Length ({quote.Open.Length} != {cnt})");
		if (quote.Close.Length != cnt) throw Err($"Quote.Close.Length != Result.Timestamp.Length ({quote.Close.Length} != {cnt})");
		if (quote.Low.Length != cnt) throw Err($"Quote.Low.Length != Result.Timestamp.Length ({quote.Low.Length} != {cnt})");
		//if (adjClose.Adjclose.Length != cnt) throw Err($"AdjClose.Adjclose.Length != Result.Timestamp.Length ({adjClose.Adjclose.Length} != {cnt})");


		var resultClean = result.RemoveInvalidDays_And_FillInAdjCloseIfMissing();
		if (resultClean.Timestamp.Length == 0) throw Err($"Result.Timestamp.Length == 0 (After cleaning. It was: {cnt} before)");

		if (result.Indicators.Adjclose != null!)
		{
			if (result.Indicators.Adjclose.Length != 1) throw Err("result.Indicators.Adjclose.Length != 1");
			var adjClose = result.Indicators.Adjclose[0];
			if (adjClose.Adjclose == null!) throw Err("AdjClose.Adjclose == null");
			if (adjClose.Adjclose.Length != cnt) throw Err($"AdjClose.Adjclose.Length != Result.Timestamp.Length ({adjClose.Adjclose.Length} != {cnt})");
		}


		return resultClean;
	}
}
