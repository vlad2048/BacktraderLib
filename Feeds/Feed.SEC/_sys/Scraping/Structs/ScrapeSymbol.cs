namespace Feed.SEC._sys.Scraping.Structs;

sealed record ScrapeSymbol(
	string FullName,
	string ShortName,
	string Exchange
)
{
	public object ToDump() => new
	{
		FullName,
		ShortName,
		Exchange,
		Code = $"""new ScrapeSymbol("{FullName}", "{ShortName}", "{Exchange}")""",
	};
}