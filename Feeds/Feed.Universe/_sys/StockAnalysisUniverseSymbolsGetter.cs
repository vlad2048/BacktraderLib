using System.Text.Json;
using BaseUtils;
using HtmlAgilityPack;

namespace Feed.Universe._sys;

static class StockAnalysisUniverseSymbolsGetter
{
	static readonly HttpClient client = new();

	public static StockAnalysisSymbol[] Scrape(IUniverse universe)
	{
		if (!Consts.StockAnalysis.FetchLimiter(universe).IsFetchNeeded())
			return JsonData.Read(universe);

		StockAnalysisSymbol[] results;

		switch (universe)
		{
			case ExchangeUniverse { Name: ExchangeName.NASDAQ }:
				results = ScrapeQuery("https://stockanalysis.com/api/screener/s/f?m=marketCap&s=desc&c=no,s,n,marketCap,price,change,revenue&cn=500&f=exchange-is-NASDAQ&i=stocks");
				break;

			case ExchangeUniverse { Name: ExchangeName.NYSE }:
				results = ScrapeQuery("https://stockanalysis.com/api/screener/s/f?m=marketCap&s=desc&c=no,s,n,marketCap,price,change,revenue&cn=500&f=exchange-is-NYSE&i=stocks");
				break;

			case IndexUniverse { Name: IndexName.SP500 }:
				results = ScrapePage("https://stockanalysis.com/list/sp-500-stocks/");
				break;

			case IndexUniverse { Name: IndexName.NASDAQ100 }:
				results = ScrapePage("https://stockanalysis.com/list/nasdaq-100-stocks/");
				break;

			case IndexUniverse { Name: IndexName.DowJones }:
				results = ScrapePage("https://stockanalysis.com/list/dow-jones-stocks/");
				break;

			default:
				throw new ArgumentException($"Unknown UniverseName: {universe}");
		}

		JsonData.Write(universe, results);

		Consts.StockAnalysis.FetchLimiter(universe).ConfirmFetchDone();

		return results;
	}



	static StockAnalysisSymbol[] ScrapeQuery(string url)
	{
		var list = new List<StockAnalysisSymbol>();
		list.AddRange(ScrapeQueryPage(url, 1, out var totalCount));
		var pageCount = FillingDiv(totalCount, 500);
		for (var i = 2; i <= pageCount; i++)
			list.AddRange(ScrapeQueryPage(url, i, out _));
		return list.DistinctBy(e => e.Symbol).ToArray();
	}

	static StockAnalysisSymbol[] ScrapeQueryPage(string url, int page, out int totalCount)
	{
		using var response = client.GetAsync($"{url}&p={page}").Result;
		if (!response.IsSuccessStatusCode) throw new ArgumentException($"Error querying {url}. {response.StatusCode}");
		var text = response.Content.ReadAsStringAsync().Result;
		var data = JsonQuery.Deser<DataFile>(text);
		totalCount = data.Data.ResultsCount;
		return data.Data.Data.SelectA(e => new StockAnalysisSymbol(e.S, e.MarketCap, e.Revenue));
	}




	static StockAnalysisSymbol[] ScrapePage(string url)
	{
		using var response = client.GetAsync(url).Result;
		if (!response.IsSuccessStatusCode) throw new ArgumentException($"Error querying {url}. {response.StatusCode}");
		var html = response.Content.ReadAsStringAsync().Result;
		var doc = new HtmlDocument();
		doc.LoadHtml(html);
		var symbols = doc.DocumentNode.SelectNodes("//tr[@class = 'svelte-utsffj']")
			.Where(e => e.SelectSingleNode("td[@class = 'sym svelte-utsffj']/a[contains(@href, '/stocks/')]") != null!)
			.SelectA(e => new StockAnalysisSymbol(
				e.SelectSingleNode("td[@class = 'sym svelte-utsffj']/a[contains(@href, '/stocks/')]").InnerText,
				ParseValue(e.SelectSingleNode("td[@class = 'svelte-utsffj'][2]").InnerText),
				ParseValue(e.SelectSingleNode("td[@class = 'tr svelte-utsffj']").InnerText)
			));
		return symbols;
	}




	// ReSharper disable ClassNeverInstantiated.Local
	sealed record DataFile(DataWrapper Data);
	sealed record DataWrapper(Symbol[] Data, int ResultsCount);
	sealed record Symbol(string S, decimal MarketCap, decimal Revenue);
	// ReSharper restore ClassNeverInstantiated.Local



	static class JsonData
	{
		static readonly JsonSerializerOptions jsonOpt = new()
		{
			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1,
		};

		public static void Write(IUniverse universe, StockAnalysisSymbol[] constituents) => File.WriteAllText(Consts.StockAnalysis.DataFile(universe), JsonSerializer.Serialize(constituents, jsonOpt));

		public static StockAnalysisSymbol[] Read(IUniverse universe) =>
			File.Exists(Consts.StockAnalysis.DataFile(universe)) switch
			{
				false => Array.Empty<StockAnalysisSymbol>(),
				true => JsonSerializer.Deserialize<StockAnalysisSymbol[]>(File.ReadAllText(Consts.StockAnalysis.DataFile(universe)), jsonOpt)!,
			};
	}

	static class JsonQuery
	{
		static readonly JsonSerializerOptions jsonOpt = new()
		{
			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};

		public static T Deser<T>(string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;
	}




	static decimal ParseValue(string str)
	{
		if (str.Length == 0) throw new ArgumentException($"Cannot parse '{str}' as a decimal (0 length)");
		var suffix = str[^1];
		if (suffix == 'B')
		{
			var subStr = str[..^1];
			if (!decimal.TryParse(subStr, out var val)) throw new ArgumentException($"Cannot parse '{subStr}' as a decimal");
			return val * 1_000_000_000;
		}
		else if (suffix == 'M')
		{
			var subStr = str[..^1];
			if (!decimal.TryParse(subStr, out var val)) throw new ArgumentException($"Cannot parse '{subStr}' as a decimal");
			return val * 1_000_000;
		}
		else
		{
			var subStr = str;
			if (!decimal.TryParse(subStr, out var val)) throw new ArgumentException($"Cannot parse '{subStr}' as a decimal");
			return val;
		}
	}



	static int FillingDiv(int a, int b)
	{
		if (b == 0) throw new ArgumentException();
		if (a < 0)
			return 0;
		var res = (a - 1) / b + 1;
		return res;
	}
}