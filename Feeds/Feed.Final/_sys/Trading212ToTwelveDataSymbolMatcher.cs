using BaseUtils;
using Feed.Final._sys.Structs;
using Feed.Symbology;

namespace Feed.Final._sys;

static class Trading212ToTwelveDataSymbolMatcher
{
	public static SymbologyData Clean(this SymbologyData data) =>
		data with
		{
			Trading212 = data.Trading212 with
			{
				Symbols = data.Trading212.Symbols							// 14781
					.WhereNot(e => e.Ticker is "TSIl_EQ" or "BRSDl_EQ")		//    -2	Trading212Symbol (Isin, Exchange, Ccy) is not unique
					.WhereNot(e => e.Exchange is "OTC Markets")				// -1569	Not sure how to map those to TwelveData.MicCode
					.Where(e => e.Type == "STOCK")							// -3905
					.ToArray(),

			},
			TwelveData = data.TwelveData				// 154874
				.WhereNot(e => e.FigiCode == "")		// -20320
				.Where(e => e.Type == "Common Stock")	//  -3729
				.ToArray(),
			Mics = data.Mics,	// 2756,
		};


	public static SymbologyData Check(this SymbologyData data)
	{
		var xs = data.Trading212.Symbols;
		var ys = data.TwelveData;
		var ms = data.Mics;

		xs.EnsureUniqueBy(e => e.Ticker);
		xs.EnsureUniqueBy(e => (e.Isin, e.Exchange, e.Ccy));
		xs.EnsureUniqueBy(e => (e.ShortName, e.Exchange, e.Ccy));

		ys.Where(e => e.FigiCode != "").EnsureUniqueBy(e => e.FigiCode);
		ys.EnsureUniqueBy(e => (e.Symbol, e.MicCode));
		CheckForeignKey(ys, y => y.MicCode, ms, m => m.Name);

		ms.EnsureUniqueBy(e => e.Name);
		ms.EnsureEquivalent(e => e.IsRoot, e => e.OperatingMic == e.Name);

		return data;
	}


	public static IReadOnlyDictionary<Trading212Symbol, TwelveDataSymbol[]> Match(this SymbologyData data)
	{
		var xs = data.Trading212.Symbols;
		var ys = data.TwelveData;

		var map = ys.GroupInDictNested(
			e => e.Exchange,
			e => e.GroupInDict(f => f.Symbol)
		);

		return (
			from x in xs
			let xExchange = Trading212Exchange_2_TwelveDataExchange[x.Exchange]
			let xMics = Trading212Exchange_2_TwelveDataMic[x.Exchange]
			where map[xExchange].ContainsKey(x.ShortName)
			let ysPotentialMatches = (
				from y in map[Trading212Exchange_2_TwelveDataExchange[x.Exchange]][x.ShortName]
				where xMics.Any(xMic => y.MicCode == xMic)
				select y
			).ToArray()
			select new
			{
				x,
				ysPotentialMatches,
			}
		).ToDictionary(e => e.x, e => e.ysPotentialMatches);
	}

	public static Symbol[] ToSymbols(this IReadOnlyDictionary<Trading212Symbol, TwelveDataSymbol[]> matches) =>
		(
			from match in matches
			where match.Value.Length == 1
			select Symbol.Make(match.Key, match.Value[0])
		)
		.ToArray();


	static IReadOnlyDictionary<K, T2> GroupInDictNested<T1, T2, K>(this IEnumerable<T1> source, Func<T1, K> funKey, Func<IEnumerable<T1>, T2> funVal) where K : notnull => source.GroupBy(funKey).ToDictionary(e => e.Key, e => funVal(e));


	static readonly Dictionary<string, string> Trading212Exchange_2_TwelveDataExchange = new()
	{
		{
			"Gettex",
			"Munich"
		},
		{
			"Bolsa de Madrid",
			"BME"
		},
		{
			"Borsa Italiana",
			"MTA"
		},
		{
			"Deutsche Börse Xetra",
			"XETR"
		},
		{
			"Euronext Amsterdam",
			"Euronext"
		},
		{
			"Euronext Brussels",
			"Euronext"
		},
		{
			"Euronext Lisbon",
			"Euronext"
		},
		{
			"Euronext Paris",
			"Euronext"
		},
		{
			"London Stock Exchange",
			"LSE"
		},
		{
			"London Stock Exchange AIM",
			"LSE"
		},
		{
			"NASDAQ",
			"NASDAQ"
		},
		{
			"NYSE",
			"NYSE"
		},
		{
			"SIX Swiss Exchange",
			"SIX"
		},
		{
			"Toronto Stock Exchange",
			"TSX"
		},
		{
			"Wiener Börse",
			"VSE"
		},
	};


	static readonly Dictionary<string, string[]> Trading212Exchange_2_TwelveDataMic = new()
	{
		{
			"Gettex",
			["XMUN"]
		},
		{
			"Bolsa de Madrid",
			["XMAD"]
		},
		{
			"Borsa Italiana",
			["XMIL"]
		},
		{
			"Deutsche Börse Xetra",
			["XETR"]
		},
		{
			"Euronext Amsterdam",
			["XAMS"]
		},
		{
			"Euronext Brussels",
			["XBRU"]
		},
		{
			"Euronext Lisbon",
			["XLIS"]
		},
		{
			"Euronext Paris",
			["XPAR"]
		},
		{
			"London Stock Exchange",
			["XLON"]
		},
		{
			"London Stock Exchange AIM",
			["AIMX"]
		},
		{
			"NASDAQ",
			["XNAS", "XNCM", "XNGS", "XNMS"]
		},
		{
			"NYSE",
			["XNYS", "ARCX", "XASE"]
		},
		{
			"SIX Swiss Exchange",
			["XSWX"]
		},
		{
			"Toronto Stock Exchange",
			["XTSE"]
		},
		{
			"Wiener Börse",
			["XWBO"]
		},
	};




	static void CheckForeignKey<X, Y, K>(IEnumerable<X> xs, Func<X, K> xFun, IEnumerable<Y> ys, Func<Y, K> yFun)
	{
		var ySet = ys.EnsureUniqueAndMakeHashSet(yFun);
		if (xs.Select(xFun).Any(e => !ySet.Contains(e)))
			throw new ArgumentException($"CheckForeignKey failed for {typeof(X).Name} -> {typeof(Y).Name}");
	}

	static HashSet<K> EnsureUniqueAndMakeHashSet<X, K>(this IEnumerable<X> xs, Func<X, K> xFun)
	{
		var arr = xs.SelectA(xFun);
		var set = arr.ToHashSet();
		if (set.Count != arr.Length)
			throw new ArgumentException($"EnsureUniqueAndMakeHashSet failed for {typeof(X).Name}");
		return set;
	}
}