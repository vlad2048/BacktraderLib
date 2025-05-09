using BaseUtils;
using Change = Feed.Final.CompanyHistoryChange;
using Graph = Feed.Final.CompanyHistoryGraph;

namespace Feed.Final;

public sealed record SecCompanySummary(
	HashSet<Quarter> Quarters
);

public sealed record SecSummary(
	Change[] Changes,
	IReadOnlyDictionary<string, SecCompanySummary> Companies,
	Graph[] Graphs
	//string[] CurrentCompanies
);