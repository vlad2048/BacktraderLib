using BaseUtils;
using Microsoft.Playwright;
using ScrapeUtils;

namespace Feed.Trading212._sys;

sealed class QuarterMan
{
	readonly DataFiller dataFiller;
	readonly ReportType reportType;
	readonly Dictionary<Quarter, ILocator> locMap;
	readonly HashSet<Quarter> xsTodo;

	public QuarterMan(
		DataFiller dataFiller,
		ReportType reportType,
		LocItem<Quarter>[] quartersPageItems,
		QuarterSet quartersDoneMap
	)
	{
		this.dataFiller = dataFiller;
		this.reportType = reportType;
		var quartersPage = quartersPageItems.SelectA(e => e.Item);
		locMap = quartersPageItems.ToDictionary(e => e.Item, e => e.Loc);

		var quartersDone = quartersDoneMap.TryGetValue(reportType, out var quartersDone_) switch
		{
			true => quartersDone_,
			false => [],
		};
		xsTodo = quartersPage.Where(e => !quartersDone.Contains(e)).ToHashSet();
	}

	public Quarter[] GetQuartersToClick()
	{
		var xsDone = dataFiller.GetReceivedQuarters(reportType);
		return xsTodo.WhereA(e => !xsDone.Contains(e));
	}

	public ILocator GetLoc(Quarter quarter) => locMap[quarter];
}