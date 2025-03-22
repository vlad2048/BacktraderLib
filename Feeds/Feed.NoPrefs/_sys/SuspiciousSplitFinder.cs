namespace Feed.NoPrefs._sys;

sealed record SuspiciousSplit(
	string Symbol,
	PriceSplit Split,
	double PricePrev,
	double PriceNext,
	Stats Stats
)
{
	public object ToDump() => new
	{
		Symbol,
		SplitExDate = Split.ExDate,
		SplitTo = Split.ToFactor,
		SplitFor = Split.ForFactor,
		PricePrev,
		PriceNext,
		Stats.PriceJumpZScore,
		JumpSuspicious = Stats.IsPriceJumpSuspicious,
		Stats.PriceRSplit,
		SplitNotSuspicious = Stats.IsPriceRSplitNotSuspicious,
	};
}

sealed record Stats(
	double PriceJumpZScore,
	double PriceRSplit
)
{
	public static readonly Stats Empty = new(double.NaN, double.NaN);
	public bool IsPriceJumpSuspicious => PriceJumpZScore >= 3;
	public bool IsPriceRSplitNotSuspicious => Math.Abs(PriceRSplit - 1) <= 0.3;
}


static class SuspiciousSplitFinder
{
	public static SuspiciousSplit[] FindSuspiciousSplits(this Price price)
	{
		price = price.Adjust();

		var (priceRStdDev, priceRAvg) = price.CalcPriceRStdDevAndAvg();
		if (double.IsNaN(priceRStdDev)) return Array.Empty<SuspiciousSplit>();
		var indexMap = price.Bars.Select(e => e.Date).Index().ToDictionary(e => e.Item, e => e.Index);
		var list = new List<SuspiciousSplit>();

		foreach (var split in price.Splits)
		{
			// Split is invalid
			if (split.IsInvalid())
			{
				var (pricePrev, priceNext) = (indexMap.TryGetValue(split.ExDate, out var t) && t > 1) switch
				{
					true => ((double)price.Bars[t - 1].Close, (double)price.Bars[t].Close),
					false => (double.NaN, double.NaN),
				};
				var stats = CalcStats(pricePrev, priceNext, priceRAvg, priceRStdDev, split);
				list.Add(new SuspiciousSplit(price.Symbol, split, pricePrev, priceNext, stats));
			}
			else
			{
				// Split is outside of the price dates or the first one
				if (!indexMap.TryGetValue(split.ExDate, out var t) || t == 0)
					continue;

				var (pricePrev, priceNext) = ((double)price.Bars[t - 1].Close, (double)price.Bars[t].Close);
				var stats = CalcStats(pricePrev, priceNext, priceRAvg, priceRStdDev, split);

				if (stats.IsPriceJumpSuspicious && stats.IsPriceRSplitNotSuspicious)
				{
					list.Add(new SuspiciousSplit(price.Symbol, split, pricePrev, priceNext, stats));
				}
			}
		}

		return [..list];
	}


	static Stats CalcStats(double pricePrev, double priceNext, double priceRAvg, double priceRStdDev, PriceSplit split)
	{
		if (double.IsNaN(pricePrev))
			return Stats.Empty;

		var priceR = priceNext / pricePrev;
		var priceJumpZScore = Math.Abs((priceR - priceRAvg) / priceRStdDev);

		var splitR = (double)(split.ToFactor / split.ForFactor);
		var priceRSplit = priceR / splitR;

		return new Stats(priceJumpZScore, priceRSplit);
	}


	static bool IsInvalid(this PriceSplit split) => split.ForFactor <= 0 || split.ToFactor <= 0 || split.ForFactor == split.ToFactor;


	static (double, double) CalcPriceRStdDevAndAvg(this Price price)
	{
		var dates = price.Splits.Select(e => e.ExDate).ToHashSet();
		var list = new List<double>();
		for (var i = 1; i < price.Bars.Length; i++)
		{
			if (dates.Contains(price.Bars[i].Date)) continue;
			var pricePrev = price.Bars[i - 1].Close;
			var priceNext = price.Bars[i].Close;
			list.Add((double)(priceNext / pricePrev));
		}

		if (list.Count <= 2)
			return (double.NaN, double.NaN);

		var avg = list.Average();
		var sum = 0.0;
		foreach (var elt in list)
		{
			var diff = elt - avg;
			sum += diff * diff;
		}
		return (sum / (list.Count - 1), avg);
	}
}