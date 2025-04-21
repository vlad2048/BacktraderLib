namespace Feed.Trading212._sys.Structs;

enum FlagAsErrorReason
{
	ReachedMaxTries,
	CompanyNotFound,
}


interface IErrorResponse;

sealed record NoneErrorResponse : IErrorResponse;
sealed record StopImmediatlyErrorResponse : IErrorResponse;
sealed record FlagAsErrorErrorResponse(ScrapeError Error, FlagAsErrorReason Reason) : IErrorResponse;
sealed record WaitAndRetryErrorResponse(ScrapeError Error, int DelaySeconds) : IErrorResponse;

static class ErrorResponse
{
	public static readonly IErrorResponse None = new NoneErrorResponse();
	public static readonly IErrorResponse StopImmediatly = new StopImmediatlyErrorResponse();
	public static IErrorResponse FlagAsError(ScrapeError error, FlagAsErrorReason reason) => new FlagAsErrorErrorResponse(error, reason);
	public static IErrorResponse WaitAndRetry(ScrapeError error, int delaySeconds) => new WaitAndRetryErrorResponse(error, delaySeconds);
}
