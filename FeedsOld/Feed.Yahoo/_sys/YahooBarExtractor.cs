namespace Feed.Yahoo._sys;

static class YahooBarExtractor
{
	public static YahooBar[] ExtractBars(this YahooResult result, Adjust adjust)
	{
		var list = new List<YahooBar>();
		for (var i = 0; i < result.Timestamp.Length; i++)
		{
			var barSrc = new BarSrc(
				result.Timestamp[i],
				(double)result.Indicators.Quote[0].Open[i]!.Value,
				(double)result.Indicators.Quote[0].Close[i]!.Value,
				(double)result.Indicators.Adjclose[0].Adjclose[i]!.Value,
				(double)result.Indicators.Quote[0].High[i]!.Value,
				(double)result.Indicators.Quote[0].Low[i]!.Value,
				result.Indicators.Quote[0].Volume[i]!.Value
			);
			YahooBar barDst;

			switch (adjust)
			{
				case Adjust.All:
					var ratio = barSrc.CloseAdj / barSrc.Close;
					barDst = new YahooBar(
						barSrc.Date,
						barSrc.Open * ratio,
						barSrc.CloseAdj,
						barSrc.High * ratio,
						barSrc.Low * ratio,
						barSrc.Volume
					);
					break;

				case Adjust.Splits:
					barDst = new YahooBar(
						barSrc.Date,
						barSrc.Open,
						barSrc.Close,
						barSrc.High,
						barSrc.Low,
						barSrc.Volume
					);
					break;

				default:
					throw new ArgumentException($"Yahoo does not recognize Adjust.{adjust}");
			}

			list.Add(barDst);
		}
		return [.. list];
	}

	sealed record BarSrc(
		DateTime Date,
		double Open,
		double Close,
		double CloseAdj,
		double High,
		double Low,
		long Volume
	);
}