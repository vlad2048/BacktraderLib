namespace Feed.Yahoo._sys;

sealed record YahooResponse(YahooChart Chart);

sealed record YahooChart(YahooResult[] Result, YahooError? Error);

sealed record YahooError(string Code, string Description);

sealed record YahooResult(
	YahooMeta Meta,
	DateTime[] Timestamp,
	DateTime[]? TimestampInvalid,
	YahooIndicators Indicators
);

sealed record YahooMeta(
	string Currency,
	string Symbol,
	string ExchangeName,
	string FullExchangeName,
	string InstrumentType,
	DateTime? FirstTradeDate,
	DateTime? RegularMarketTime,
	bool HasPrePostMarketData,
	long Gmtoffset,
	string Timezone,
	string ExchangeTimezoneName,
	decimal RegularMarketPrice,
	decimal FiftyTwoWeekHigh,
	decimal FiftyTwoWeekLow,
	decimal RegularMarketDayHigh,
	decimal RegularMarketDayLow,
	long RegularMarketVolume,
	string LongName,
	string ShortName,
	decimal ChartPreviousClose,
	int? PriceHint,
	YahooCurrentTradingPeriod CurrentTradingPeriod,
	string DataGranularity,
	string Range,
	string[] ValidRanges
);

sealed record YahooCurrentTradingPeriod(TradingPeriod Pre, TradingPeriod Regular, TradingPeriod Post);

sealed record TradingPeriod(string Timezone, DateTime End, DateTime Start, long Gmtoffset);

sealed record YahooIndicators(
	YahooQuote[] Quote,
	YahooAdjClose[] Adjclose
);

sealed record YahooQuote(
	long?[] Volume,
	decimal?[] High,
	decimal?[] Open,
	decimal?[] Close,
	decimal?[] Low
);

sealed record YahooAdjClose(decimal?[] Adjclose);


static class YahooStructsUtils
{
	public static YahooResult RemoveInvalidDays_And_FillInAdjCloseIfMissing(this YahooResult result)
	{
		var listTimestampInvalid = new List<DateTime>();

		var listTimestamp = new List<DateTime>();
		var listVolume = new List<long?>();
		var listHigh = new List<decimal?>();
		var listOpen = new List<decimal?>();
		var listClose = new List<decimal?>();
		var listLow = new List<decimal?>();
		var listAdjClose = new List<decimal?>();

		var quote = result.Indicators.Quote[0];
		//var adjCloseArr = result.Indicators.Adjclose[0];
		var adjCloseArr = result.Indicators.Adjclose switch
		{
			not null => result.Indicators.Adjclose[0].Adjclose,
			null => quote.Close,
		};

		for (var i = 0; i < result.Timestamp.Length; i++)
		{
			var timestamp = result.Timestamp[i];
			var volume = quote.Volume[i];
			var high = quote.High[i];
			var open = quote.Open[i];
			var close = quote.Close[i];
			var low = quote.Low[i];
			var adjClose = adjCloseArr[i];

			if (
				volume.HasValue &&
				high.HasValue &&
				open.HasValue &&
				close.HasValue &&
				low.HasValue &&
				adjClose.HasValue
			)
			{
				listTimestamp.Add(timestamp);
				listVolume.Add(volume);
				listHigh.Add(high);
				listOpen.Add(open);
				listClose.Add(close);
				listLow.Add(low);
				listAdjClose.Add(adjClose);
			}
			else
			{
				listTimestampInvalid.Add(timestamp);
			}
		}

		return result with
		{
			Timestamp = [.. listTimestamp],

			TimestampInvalid = [.. listTimestampInvalid],

			// ReSharper disable once WithExpressionModifiesAllMembers
			Indicators = result.Indicators with
			{
				Quote =
				[
					new YahooQuote(
						[..listVolume],
						[..listHigh],
						[..listOpen],
						[..listClose],
						[..listLow]
					),
				],
				Adjclose =
				[
					new YahooAdjClose(
						[..listAdjClose]
					),
				]
			}
		};
	}
}