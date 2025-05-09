using Feed.TwelveData._sys;

namespace Feed.TwelveData;

public static class API
{
	static string? apiKey { get; set; }
	internal static string ApiKey => apiKey ?? throw new ArgumentException("Feed.TwelveData.API.Init(string apiKey) first");

	public static void Init(string apiKey_) => apiKey = apiKey_;

	public static TwelveDataSymbol[] FetchSymbols() => SymbolFetcher.Fetch();
}