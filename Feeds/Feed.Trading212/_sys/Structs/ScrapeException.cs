namespace Feed.Trading212._sys.Structs;

public sealed class ScrapeException(ScrapeError error) : Exception(error.Message, error.InnerException)
{
	public ScrapeError Error => error;
}
