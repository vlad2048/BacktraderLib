using BaseUtils;

namespace Feed.Trading212._sys.Structs;

public enum ScrapeErrorType
{
	RateLimit,
	NoInternet,
	UnexpectedRequestException,
	SymbolNotFound,
	QuartersMissing,
};

public sealed class ScrapeException(ScrapeErrorType errorType, string message, Exception? innerException = null) : Exception(message, innerException)
{
	public ScrapeErrorType ErrorType => errorType;

	public static ScrapeException RateLimit => new(ScrapeErrorType.RateLimit, "RateLimit");
	public static ScrapeException NoInternet => new(ScrapeErrorType.NoInternet, "NoInternet");
	public static ScrapeException UnexpectedRequestException(Exception innerException) => new(ScrapeErrorType.UnexpectedRequestException, $"UnexpectedRequestException: {innerException.Message}", innerException);
	public static ScrapeException SymbolNotFound(CompanySearchInfo nfo) => new(ScrapeErrorType.SymbolNotFound, $"Company not found. Name={nfo.SecCompanyName} Ticker={nfo.Ticker} Exchange={nfo.Exchange}");
	public static ScrapeException QuartersMissing(ReportType reportType, Quarter[] quarters) => new(ScrapeErrorType.QuartersMissing, $"Missed quarters for {reportType}: {quarters.FmtHuman()}");
}
