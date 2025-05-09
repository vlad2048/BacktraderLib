namespace Feed.Trading212;

public enum CompanyScrapeStatus
{
	/// <summary>
	/// Scraping finished successfully
	/// </summary>
	Success,

	/// <summary>
	/// Scraping finished with an error or was cancelled
	/// </summary>
	InProgress,

	/// <summary>
	/// Scraping failed too many times, we give up on this one
	/// </summary>
	Error,
}

public sealed record CompanyScrapeState(
	CompanyScrapeStatus Status,
	DateTime LastScrapeTime,
	string? Error
);