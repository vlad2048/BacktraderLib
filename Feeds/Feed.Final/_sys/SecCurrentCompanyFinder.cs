using BacktraderLib;
using BaseUtils;
using LINQPad;
using Change = Feed.Final.CompanyHistoryChange;

namespace Feed.Final._sys;

static class SecCurrentCompanyFinder
{
	public static string[] Find(SecSummary summary, bool log = false)
	{
		var reportCount = summary.CountReports().PlotReportCount(log);
		var cutoff = reportCount.FindCutoff();
		var xsCurrent = summary.Companies.Where(kv => kv.Value.Quarters.Any(e => e >= cutoff)).ToHashSet(kv => kv.Key);
		var xsResuscitated = summary.FindResuscitated();
		var xsFormer = summary.Changes.ToHashSet(e => e.NamePrev);
		var cntFormer = xsFormer.Count;
		xsFormer = xsFormer.RemoveIfIn(xsResuscitated, out var cntFormerButResuscitated);

		var cntCurrent = xsCurrent.Count;
		xsCurrent = xsCurrent.RemoveIfIn(xsFormer, out var cntCurrentButFormer);

		if (log)
		{
			// @formatter:off
			$"""
			-> cutoff = {cutoff}
			
			former      {cntFormer}
			            -{cntFormerButResuscitated.right("resuscitated")}
			            -----------
			            = {xsFormer.Count}
			
			current      {cntCurrent}
			            -{cntCurrentButFormer.right("former")}
			            ------------
			            = {xsCurrent.Count}
			
			""".Dump();
			// @formatter:on
		}

		return [..xsCurrent];
	}

	static string right(this int e, string str) => $"{e}".PadRight(16) + str;

	static HashSet<string> RemoveIfIn(this HashSet<string> source, HashSet<string> dels, out int cntDel)
	{
		cntDel = source.Count(dels.Contains);
		return source.Where(e => !dels.Contains(e)).ToHashSet();
	}


	static IReadOnlyDictionary<Quarter, int> CountReports(this SecSummary summary)
	{
		var xs = summary.Companies.Values.SelectMany(e => e.Quarters).Distinct().OrderBy(e => e).ToArray();
		return new SortedDictionary<Quarter, int>(
			xs.ToDictionary(
				e => e,
				x => summary.Companies.Values.Count(e => e.Quarters.Contains(x))
			)
		);
	}

	static IReadOnlyDictionary<Quarter, int> PlotReportCount(this IReadOnlyDictionary<Quarter, int> reportCount, bool log)
	{
		if (!log) return reportCount;
		Plot.Make([
			new BarTrace
			{
				X = reportCount.SelectA(e => $"{e.Key}"),
				Y = reportCount.SelectA(e => $"{e.Value}")
			}
		]).Dump("SEC Filings per Quarter");
		"-> So if we have more than 4000 reports in 2 consecutive quarters we pick the first of those as the cutoff\n".Dump();
		return reportCount;
	}

	static Quarter FindCutoff(this IReadOnlyDictionary<Quarter, int> reportCount)
	{
		var idx = reportCount.Index().Reverse().First(e => e.Item.Value >= 4000).Index - 1;
		return reportCount.ElementAt(idx).Key;
	}


	static HashSet<string> FindResuscitated(this SecSummary summary)
	{
		var dels = summary.Changes.GroupInDict(e => e.NamePrev).TidyGroup();
		var adds = summary.Changes.GroupInDict(e => e.NameNext).TidyGroup();
		bool IsResucitated(string name)
		{
			if (!dels.TryGetValue(name, out var delsChgs)) return false;
			var lastDel = delsChgs.Last();
			if (!adds.TryGetValue(name, out var addsChgs)) return false;
			var lastAdd = addsChgs.Last();
			return lastAdd.Date >= lastDel.Date;
		}
		return adds.Keys.Where(IsResucitated).ToHashSet();
	}





	static IReadOnlyDictionary<string, Change[]> TidyGroup(this IReadOnlyDictionary<string, Change[]> grp) => grp.ToDictionary(kv => kv.Key, kv => kv.Value.OrderBy(e => e.Date).Distinct().ToArray());
}