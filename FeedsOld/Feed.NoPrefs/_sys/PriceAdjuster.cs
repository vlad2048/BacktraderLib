namespace Feed.NoPrefs._sys;

static class PriceAdjuster
{
	public static Price Adjust(this Price price, bool adjustDividends, bool adjustSplits)
	{
		var bars = price.Bars.ToArray();

		if (adjustDividends)
		{
			foreach (var dividend in Enumerable.Reverse(price.Dividends))
				for (var i = 0; i < bars.Length; i++)
				{
					if (bars[i].Date < dividend.ExDate)
					{
						bars[i] = bars[i] with
						{
							Open = bars[i].Open - dividend.Amount,
							High = bars[i].High - dividend.Amount,
							Low = bars[i].Low - dividend.Amount,
							Close = bars[i].Close - dividend.Amount,
						};
					}
				}
		}

		if (adjustSplits)
		{
			foreach (var split in Enumerable.Reverse(price.Splits))
			{
				var splitRatio = split.ToFactor / split.ForFactor;
				for (var i = 0; i < bars.Length; i++)
				{
					if (bars[i].Date < split.ExDate)
					{
						bars[i] = bars[i] with
						{
							Open = bars[i].Open / splitRatio,
							High = bars[i].High / splitRatio,
							Low = bars[i].Low / splitRatio,
							Close = bars[i].Close / splitRatio,
							Volume = (long)(bars[i].Volume * splitRatio),
						};
					}
				}
			}
		}

		return price with { Bars = bars };
	}
}