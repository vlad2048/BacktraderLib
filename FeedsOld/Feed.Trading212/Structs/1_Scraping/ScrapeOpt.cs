namespace Feed.Trading212;

public sealed class ScrapeOpt
{
	public bool DryRun { get; init; }
	public bool DisableSaving { get; init; }
	public TimeSpan? RefreshOldPeriod { get; init; }
}