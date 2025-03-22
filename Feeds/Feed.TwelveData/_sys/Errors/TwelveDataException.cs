using Feed.TwelveData._sys.Structs;

namespace Feed.TwelveData._sys.Errors;

sealed class TwelveDataException(
	string symbol,
	TwelveDataExceptionType type,
	Exception? httpRequestException,
	TwelveDataError? apiError
)
	: Exception($"[{symbol}] {type}  apiError:{apiError}")
{
	public string Symbol { get; } = symbol;
	public TwelveDataExceptionType Type { get; } = type;
	public Exception? HttpRequestException { get; } = httpRequestException;
	public TwelveDataError? ApiError { get; } = apiError;


	internal static TwelveDataException MakeHttpRequestException(string symbol, Exception ex) => new(
		symbol,
		TwelveDataExceptionType.HttpRequest,
		ex,
		null
	);

	internal static TwelveDataException MakeApiError(string symbol, TwelveDataError apiError) => new(
		symbol,
		TwelveDataExceptionType.ApiError,
		null,
		apiError
	);
};



enum TwelveDataExceptionType
{
	HttpRequest,
	ApiError,
}