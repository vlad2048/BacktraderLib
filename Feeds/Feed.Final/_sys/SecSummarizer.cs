using BaseUtils;
using Feed.Final._sys.CompanyHistoryPartitioning;
using Change = Feed.Final.CompanyHistoryChange;

namespace Feed.Final._sys;

static class SecSummarizer
{
	public static SecSummary Summarize()
	{
		var changes = new List<Change>();
		var companies = new Dictionary<string, SecCompanySummary>();

		SEC.API.Rows.Group.Load<SEC.SubRow>()
			//.Where(e => e.Form is "10-Q" or "10-K")
			.ForEach(e =>
			{
				if (e.Former != null && e.Changed != null && e.Former != e.Name)
					changes.Add(new Change(
						e.Former!,
						e.Name,
						e.Changed!.Value
					));

				if (!companies.TryGetValue(e.Name, out var company))
					company = companies[e.Name] = new SecCompanySummary([]);
				company.Quarters.Add(e.Quarter);
			});

		var changesArr = changes
			.Distinct()
			.OrderBy(e => e.Date)
			.ToArray();
		var graphs = changesArr.Partition();

		return new SecSummary(
			changesArr,
			companies,
			graphs
		);
	}

	public static Change[] Fetch() => (
			from subRow in SEC.API.Rows.Group.Load<SEC.SubRow>()
			where
				subRow.Former != null &&
				subRow.Changed != null &&
				subRow.Former != subRow.Name
			select new Change(
				subRow.Former!,
				subRow.Name,
				subRow.Changed!.Value
			)
		)
		.Distinct()
		.OrderBy(e => e.Date)
		.ToArray();
}