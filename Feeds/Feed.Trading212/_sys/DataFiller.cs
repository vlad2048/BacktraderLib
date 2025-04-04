using BaseUtils;
using Feed.Trading212._sys.Utils;
using Microsoft.Playwright;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Web;
using LINQPad;

namespace Feed.Trading212._sys;

sealed class DataFiller
{
	readonly ConcurrentDictionary<ReportType, ConcurrentDictionary<Quarter, RefField[]>> reports = new();

	public Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports => reports.ToDictionary(kv => kv.Key, kv => kv.Value.ToSortedDictionary());

	public void OnRequestFinished(object? _, IRequest req)
	{
		var Log = Logger.Make(LogCategory.Request);

		try
		{
			var reportTypeOpt = Reqs.GetReportType(req);
			if (!reportTypeOpt.HasValue) return;
			var reportType = reportTypeOpt.Value;

			var quarter = Quarter.ParseScrapeQuarter(HttpUtility.ParseQueryString(req.Url)["period"] ?? throw new ArgumentException($"Could not find the period in the url: {req.Url}"));

			Task.Run(async () =>
			{
				var res = await req.ResponseAsync() ?? throw new ArgumentException("Response is null");
				var resJson = await res.JsonAsync();
				if (!resJson.HasValue) throw new ArgumentException("!resJson.HasValue");
				if (resJson.Value.ValueKind != JsonValueKind.Array) throw new ArgumentException($"Wrong JsonValueKind.  Expected:Array  Actual:{resJson.Value.ValueKind}");
				var jsonArray = JsonUtils.Deser<RefField[]>(JsonUtils.Ser(resJson.Value));

				if (!reports.TryGetValue(reportType, out var quarterMap))
					quarterMap = reports[reportType] = new ConcurrentDictionary<Quarter, RefField[]>();
				quarterMap[quarter] = jsonArray;

				Log($"    Got   {quarter}   ({reportType})");
			});
		}
		catch (Exception ex)
		{
			Log("EXCEPTION in DataFiller.OnRequestFinished");
			ex.Dump();
			throw;
		}
	}
}