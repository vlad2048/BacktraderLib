using BaseUtils;

namespace Feed.Trading212._sys.Structs;

public enum ScrapeErrorType
{
	Cancelled,
	RateLimit,
	NoInternet,
	UnexpectedException,
	UnexpectedRequestException,
	CompanyNotFound,
	QuartersMissing,
};

public sealed record ScrapeError(
	ScrapeErrorType Type,
	string Message,
	Exception? InnerException
)
{
	public static ScrapeError Cancelled => new(
		ScrapeErrorType.Cancelled,
		"Cancelled",
		null
	);

	public static ScrapeError RateLimit => new(
		ScrapeErrorType.RateLimit,
		"RateLimit",
		null
	);

	public static ScrapeError NoInternet => new(
		ScrapeErrorType.NoInternet,
		"NoInternet",
		null
	);

	public static ScrapeError UnexpectedException(Exception innerException) => new(
		ScrapeErrorType.UnexpectedException,
		$"UnexpectedException: {innerException.Message}",
		innerException
	);

	public static ScrapeError UnexpectedRequestException(Exception innerException) => new(
		ScrapeErrorType.UnexpectedRequestException,
		$"UnexpectedRequestException: {innerException.Message}",
		innerException
	);

	public static ScrapeError CompanyNotFound(CompanyDef company) => new(
		ScrapeErrorType.CompanyNotFound,
		$"Company not found. Name={company.Name} Exchange={company.Exchange} MainTicker={company.MainTicker}",
		null
	);

	public static ScrapeError QuartersMissing(ReportType reportType, Quarter[] quarters) => new(
		ScrapeErrorType.QuartersMissing,
		$"Missed quarters for {reportType}: {quarters.FmtHuman()}",
		null
	);

	public override string ToString() => $"[{Type}] {Message}" + (InnerException != null ? $"  (innerEx: {InnerException})" : "");
}
