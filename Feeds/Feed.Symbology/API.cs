using Feed.Symbology._sys;
using Feed.Symbology._sys.Structs;

namespace Feed.Symbology;

public static class API
{
	static ApiKeys? apiKeys { get; set; }
	internal static ApiKeys ApiKeys => apiKeys ?? throw new ArgumentException("Call Feed.TwelveData.API.Init() first");

	public static void Init(string trading212ApiKey, string twelveDataApiKey) => apiKeys = new ApiKeys(trading212ApiKey, twelveDataApiKey);

	public static Trading212SymbolData FetchTrading212Data() => Trading212SymbolFetcher.Fetch();
	public static TwelveDataSymbol[] FetchTwelveDataSymbols() => TwelveDataSymbolFetcher.Fetch();
	public static Mic[] FetchMics() => MicFetcher.Fetch();
}