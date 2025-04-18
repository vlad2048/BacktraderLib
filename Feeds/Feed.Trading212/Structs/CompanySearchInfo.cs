namespace Feed.Trading212;

public sealed record CompanySearchInfo(
	string SecCompanyName,
	string Ticker,
	string Exchange
)
{
	public override string ToString() => SecCompanyName;
	public object ToDump() => ToString();
}