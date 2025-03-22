using BacktraderLib._sys;
using BaseUtils;
using Frames;

namespace BacktraderLib;

public static class BuyAndHoldEquityCalc
{
	public static Serie<string> BuyAndHold(Frame<string, string, Bar> prices, int timeOfs, double cash)
	{
		var map = GetBuyAndSellTimes(prices, timeOfs);
		var syms = map.Where(kv => kv.Value != null).SelectA(kv => kv.Key);
		var cashPerSym = cash / syms.Length;
		var result = Sim.Run(
			prices,
			timeOfs,
			new BrokerOpts
			{
				Cash = cash,
				UseCurrentCloseForMarketOrders = true,
			},
			[],
			ctx =>
			{
				ctx.WhenTick.Subscribe(t =>
				{
					foreach (var sym in syms)
					{
						var (tBuy, tSel) = map[sym]!.Value;
						if (t == tBuy)
						{
							var price = prices[sym, Bar.Close].Values[t];
							var size = cashPerSym / price;
							ctx.Buy(sym, size, OrderType.Market);
						}
						else if (t == tSel)
						{
							ctx.Sell(sym, ctx.Positions[sym], OrderType.Market);
						}
					}
				});
			}
		);
		return result.Equity;
	}

	static IReadOnlyDictionary<string, (int, int)?> GetBuyAndSellTimes(Frame<string, string, Bar> prices, int timeOfs)
	{
		var map = new Dictionary<string, (int, int)?>();
		foreach (var price in prices)
		{
			var xs = price[Bar.Close].Values;
			var tBuy = xs.GetFirstNonNaNIndex(timeOfs);
			var tSel = xs.GetLastNonNaNIndex(timeOfs);
			var contig = xs.AreThereNoNaNsWithin(tBuy, tSel);

			map[price.Name] = (tBuy, tSel, contig) switch
			{
				(not null, not null, true) when tBuy < tSel => (tBuy.Value, tSel.Value),
				_ => null,
			};
		}
		return map;
	}

	static bool AreThereNoNaNsWithin(this double[] xs, int? t0, int? t1)
	{
		if (!t0.HasValue || !t1.HasValue) return false;
		var (t0v, t1v) = (t0.Value, t1.Value);
		for (var t = t0v; t < t1v; t++)
			if (xs[t].IsNaN())
				return false;
		return true;
	}

	static int? GetFirstNonNaNIndex(this double[] xs, int timeOfs)
	{
		for (var i = timeOfs; i < xs.Length; i++)
			if (xs[i].IsNotNaN())
				return i;
		return null;
	}

	static int? GetLastNonNaNIndex(this double[] xs, int timeOfs)
	{
		for (var i = xs.Length - 1; i >= timeOfs; i--)
			if (xs[i].IsNotNaN())
				return i;
		return null;
	}
}