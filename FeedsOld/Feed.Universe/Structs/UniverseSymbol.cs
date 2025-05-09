using BaseUtils;

namespace Feed.Universe;

public sealed record UniverseSymbol(
	string Symbol,
	string Country,
	string Name,
	string Currency,
	string Exchange,
	string MicCode,
	string Type,
	string FigiCode,
	string CfiCode,

	string SecCompanyName,

	decimal MarketCap,
	decimal Revenue
)
{
	public object ToDump() => new
	{
		Symbol,
		Exchange,
		Country,
		Name,
		Currency,
		Type,
		//MicCode,
		//FigiCode,
		//CfiCode,

		SecCompanyName,

		MarketCap = MarketCap.FmtHuman(),
		Revenue = Revenue.FmtHuman(),
	};
}