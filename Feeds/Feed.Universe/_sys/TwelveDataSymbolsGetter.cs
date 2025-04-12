using BaseUtils;
using System.Text.Json;

namespace Feed.Universe._sys;

static class TwelveDataSymbolsGetter
{
	public static string? ApiKey { get; set; }

	public static TwelveDataSymbol[] All => all.Value;


	static readonly Lazy<TwelveDataSymbol[]> all = new(Get);

	const string Url = "https://api.twelvedata.com/stocks";

	static TwelveDataSymbol[] Get()
	{
		if (!Consts.TwelveDataSymbols.FetchLimiter.IsFetchNeeded())
			return JsonSave.LoadOr(Consts.TwelveDataSymbols.SymbolsFile, Array.Empty<TwelveDataSymbol>());

		using var client = new HttpClient
		{
			DefaultRequestHeaders =
			{
				{ "Authorization", $"apikey {ApiKey ?? throw new ArgumentException("Set TwelveData API key first using Feed.Universe.API.SetTwelveDataApiKey(string apiKey)")}" },
			},
		};
		using var response = client.GetAsync(Url).Result;
		if (!response.IsSuccessStatusCode) throw new ArgumentException($"Error querying {Url}. {response.StatusCode}");
		var text = response.Content.ReadAsStringAsync().Result;

		var data = JsonTwelveData.Deser<DataFile>(text).Data
			.Clean()
			.EnsureUniqueBy(e => e.Key, "TwelveDataSymbols should be unique by (Symbol,Exchange,Country)");

		JsonSave.Save(Consts.TwelveDataSymbols.SymbolsFile, data);
		Consts.TwelveDataSymbols.FetchLimiter.ConfirmFetchDone();
		return data;
	}

	// This ensures that TwelveDataSymbol.Symbol are unique
	// and that Name normalization doesn't introduce duplicates

	static readonly HashSet<TwelveDataSymbolKey> forbiddenKeys =
	[
		new TwelveDataSymbolKey("ZG", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("BATRK", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("KELYB", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("UONEK", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("LBTYB", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("LBTYA", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("NWS", "NASDAQ", "United States"),
		new TwelveDataSymbolKey("BIO.B", "NYSE", "United States"),
		new TwelveDataSymbolKey("HVT.A", "NYSE", "United States"),
		new TwelveDataSymbolKey("UHAL.B", "NYSE", "United States"),
	];

	static IEnumerable<TwelveDataSymbol> Clean(this IEnumerable<TwelveDataSymbol> source) =>
		source
			.Where(e => !(e.Symbol == "ARQQW" && e.FigiCode == ""))
			.Where(e => !forbiddenKeys.Contains(e.Key));

	// ReSharper disable ClassNeverInstantiated.Local
	sealed record DataFile(TwelveDataSymbol[] Data);
	// ReSharper restore ClassNeverInstantiated.Local


	static class JsonSave
	{
		static readonly JsonSerializerOptions jsonOpt = new()
		{
			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1,
		};

		static T Deser<T>(string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;
		static string Ser<T>(T obj) => JsonSerializer.Serialize(obj, jsonOpt);

		public static T LoadOr<T>(string file, T defaultValue) => File.Exists(file) switch
		{
			false => defaultValue,
			true => Deser<T>(File.ReadAllText(file)),
		};

		public static void Save<T>(string file, T obj) => File.WriteAllText(file, Ser(obj));
	}

	static class JsonTwelveData
	{
		static readonly JsonSerializerOptions jsonOpt = new()
		{
			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1,
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		};

		public static T Deser<T>(string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;
	}
}