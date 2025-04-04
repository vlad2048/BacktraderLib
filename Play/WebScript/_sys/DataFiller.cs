using System.Collections.Concurrent;
using BaseUtils;
using Microsoft.Playwright;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Web;
using WebScript._sys.Utils;
using WebScript.Structs;

namespace WebScript._sys;

sealed class DataFiller
{
	readonly ConcurrentDictionary<ReportType, SymbolReportData> reports = new();

	public SymbolData Data => new(reports.ToDictionary());

	public void OnRequestFinished(object? _, IRequest req)
	{
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
				var jsonArray = JsonUtils.Deser<JsonArray>(JsonUtils.Ser(resJson.Value));

				if (!reports.TryGetValue(reportType, out var symbolReportData))
					symbolReportData = reports[reportType] = new SymbolReportData(new SortedDictionary<Quarter, JsonArray>());
				symbolReportData.Quarters[quarter] = jsonArray;

				Log($"    Got   {quarter}   ({reportType})");
			});
		}
		catch (Exception ex)
		{
			Log("EXCEPTION in DataFiller.OnRequestFinished");
			Log(ex);
			throw;
		}
	}
}