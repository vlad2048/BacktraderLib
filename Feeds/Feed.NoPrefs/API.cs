using BaseUtils;
using Feed.NoPrefs._sys;
using Feed.NoPrefs._sys.DoltLogic;
using FeedUtils;
using Frames;
using LINQPad;

namespace Feed.NoPrefs;

public static class API
{
	public static Frame<string, string, Bar> Fetch(string[] syms) =>
		PriceBuilder.Build(
			syms.SelectA(e => (e, FetchRaw(e).Adjust().Bars)),
			e => e.Date,
			e => (double)e.Open,
			e => (double)e.High,
			e => (double)e.Low,
			e => (double)e.Close,
			e => e.Volume
		);

	public static IReadOnlyDictionary<string, Symbol> Symbols => symbols.Value;

	public static Price FetchRaw(string symbol)
	{
		var res = Symbols.ContainsKey(symbol) switch
		{
			true => prices.TryGetValue(symbol, out var price) switch
			{
				true => price,
				false => prices[symbol] = DBs.Stocks.Load(Keepers.Price(symbol)),
			},
			false => throw new ArgumentException($"Symbol '{symbol}' not found."),
		};

		var splitsOvr = SplitOverrides.Get(symbol);
		if (splitsOvr != null)
			res = res with { Splits = splitsOvr };

		var suspiciousSplits = res.FindSuspiciousSplits();
		if (suspiciousSplits.Length > 0)
			suspiciousSplits.Dump();

		return res;
	}


	public static Frame<string, int> FetchYields() => FetchYieldsRaw().ToFrame();


	// ***********
	// * Private *
	// ***********
	static readonly Lazy<IReadOnlyDictionary<string, Symbol>> symbols = new(() => DBs.Stocks.Load(Keepers.Symbols));
	static readonly Dictionary<string, Price> prices = new();

	static Yields FetchYieldsRaw() => DBs.Rates.Load(Keepers.Yields);
}