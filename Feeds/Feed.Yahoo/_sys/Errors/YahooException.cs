using System.Net;
using System.Text.Json;

namespace Feed.Yahoo._sys.Errors;

sealed class YahooException(
	string symbol,
	YahooExceptionNfo nfo,
	string? errorFilename = null,
	string? responseString = null
)
	: Exception($"[{symbol}] {nfo}")
{
	// ReSharper disable once UnusedMember.Global
	public string Symbol { get; } = symbol;
	public YahooExceptionNfo Nfo { get; } = nfo;
	public string? ErrorFilename { get; } = errorFilename;
	public string? ResponseString { get; set; } = responseString;


	internal static YahooException MakeHttpRequestException(string symbol, Exception ex) => new(
		symbol,
		new YahooExceptionNfo(
			YahooExceptionType.HttpRequest,
			ex.GetNestedMessages()
		)
	);


	internal static YahooException MakeHttpResponseException(string symbol, HttpStatusCode responseStatusCode, YahooError error) => new(
		symbol,
		new YahooExceptionNfo(
			YahooExceptionType.HttpResponse,
			[$"HttpStatusCode:{responseStatusCode} YahooError:{error}"]
		)
	);


	internal static YahooException MakeJsonException(string symbol, JsonException ex, string responseString) => new(
		symbol,
		new YahooExceptionNfo(
			YahooExceptionType.Json,
			ex.GetNestedMessages()
		),
		Utils.GetErrorFile(symbol),
		responseString
	);


	internal static YahooException MakeInvalidResponseException(string symbol, string responseString, string message) => new(
		symbol,
		new YahooExceptionNfo(
			YahooExceptionType.InvalidResponse,
			[message]
		),
		Utils.GetErrorFile(symbol),
		responseString
	);

	internal static YahooException MakeInvalidResultException(string symbol, string message) => new(
		symbol,
		new YahooExceptionNfo(
			YahooExceptionType.InvalidResult,
			[message]
		)
	);

	internal static YahooException MakeSymbolIsIgnoredException(string symbol) => new(
		symbol,
		new YahooExceptionNfo(
			YahooExceptionType.SymbolIsIgnored,
			[]
		)
	);
}




file static class Utils
{
	public static string GetErrorFile(string symbol) => Path.Combine(Path.GetTempPath(), $"{symbol.ToSafe()}.json");

	public static string[] GetNestedMessages(this Exception ex)
	{
		var list = new List<string>();
		var cur = ex;
		while (cur != null)
		{
			list.Add(cur.GetMessage());
			cur = cur.InnerException;
		}
		return [.. list];
	}


	static string GetMessage(this Exception ex) => $"[{ex.GetType().Name}] {ex.Message}";

	static readonly string[] systemReservedNames =
	[
		"CON",
		"PRN",
	];

	static string ToSafe(this string e)
	{
		if (systemReservedNames.Any(f => e == f)) return $"{e}_";
		var pref = systemReservedNames.FirstOrDefault(f => e.StartsWith($"{f}."));
		if (pref != null) return $"{pref}_{e[pref.Length..]}";
		return e;
	}
}
