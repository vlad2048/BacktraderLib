using System.Text.Json.Serialization;
using BaseUtils;
using Feed.Final._sys.CompanyHistoryPartitioning.GraphPrinting;
using LINQPad;

namespace Feed.Final;

public sealed record CompanyHistoryChange(
	string NamePrev,
	string NameNext,
	DateOnly Date
)
{
	public object ToDump() => new
	{
		Date,
		Change = $"{NamePrev}  ->  {NameNext}",
	};
}

public sealed record CompanyHistoryPoint(
	string Name,
	DateOnly? Date
)
{
	public override string ToString() =>
		Date switch
		{
			not null => $"{Name} ({Date.Value:yyyy-MM-dd})",
			null => Name,
		};
}

public sealed record CompanyHistoryEdge(
	CompanyHistoryPoint Prev,
	CompanyHistoryPoint Next
);

public sealed record CompanyHistoryGraph(
	CompanyHistoryChange[] Changes,
	CompanyHistoryPoint[] Points,
	CompanyHistoryEdge[] Edges
)
{
	public object ToDump() => Util.VerticalRun([
		GraphPrinter.Print(this),
		Changes,
	]);

	[JsonIgnore]
	public string FullName => string.Join(" ", Points.SelectDistinctA(f => f.Name));
}