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


static class ErrorResponseUtils
{
	public static string? GetLogMessage(this IErrorResponse errorResponse) =>
		errorResponse switch
		{
			NoneErrorResponse => null,
			StopImmediatlyErrorResponse => "Cancel",
			FlagAsErrorErrorResponse { Error: var error, Reason: var reason } => $"Error => FlagAsError(reason={reason})    ({error})",
			WaitAndRetryErrorResponse { Error: var error, DelaySeconds: var delay } => $"Error => WaitAndRetry(delay={delay}sec)    ({error})",
			_ => throw new ArgumentException($"Unknown IErrorResponse: {errorResponse.GetType().Name}"),
		};

	public static Exception? GetLogException(this IErrorResponse errorResponse) =>
		errorResponse switch
		{
			NoneErrorResponse => null,
			StopImmediatlyErrorResponse => null,
			FlagAsErrorErrorResponse { Error.InnerException: var ex } => ex,
			WaitAndRetryErrorResponse { Error.InnerException: var ex } => ex,
			_ => throw new ArgumentException($"Unknown IErrorResponse: {errorResponse.GetType().Name}"),
		};
}