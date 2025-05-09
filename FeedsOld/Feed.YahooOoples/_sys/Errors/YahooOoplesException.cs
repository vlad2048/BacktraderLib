namespace Feed.YahooOoples._sys.Errors;

sealed class YahooOoplesException(string symbol, Exception ex) : Exception($"'{symbol}' -> {ex.Message}", ex)
{
	public string Symbol => symbol;
}