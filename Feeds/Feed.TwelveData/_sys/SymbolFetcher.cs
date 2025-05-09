using System.Text.Json;

namespace Feed.TwelveData._sys;

static class SymbolFetcher
{
	public static TwelveDataSymbol[] Fetch()
	{
		using var response = Client.GetAsync(Url).Result;
		if (!response.IsSuccessStatusCode) throw new ArgumentException($"Error querying {Url}. {response.StatusCode}");
		var text = response.Content.ReadAsStringAsync().Result;
		return JsonTwelveData.Deser<DataFile>(text).Data;
	}


	static readonly Lazy<HttpClient> client = new(() => new HttpClient
	{
		DefaultRequestHeaders =
		{
			{ "Authorization", $"apikey {API.ApiKey}" },
		},
	});
	static HttpClient Client => client.Value;

	const string Url = "https://api.twelvedata.com/stocks";

	
	// ReSharper disable ClassNeverInstantiated.Local
	sealed record DataFile(TwelveDataSymbol[] Data);
	// ReSharper restore ClassNeverInstantiated.Local
	

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