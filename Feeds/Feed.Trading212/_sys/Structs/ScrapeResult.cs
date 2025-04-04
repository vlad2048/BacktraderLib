using BaseUtils;

namespace Feed.Trading212._sys.Structs;

sealed record ScrapeResult(
	SymbolDef Symbol,
	string? ErrorMessage,
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
)
{
	public override string ToString() =>
		$"{Reports.Sum(e => e.Value.Count)} reports " +
		ErrorMessage switch
		{
			null => "(success)",
			not null => $"(error: {ErrorMessage})",
		};
}