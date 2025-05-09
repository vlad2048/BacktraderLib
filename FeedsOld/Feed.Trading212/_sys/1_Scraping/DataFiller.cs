using System.Collections.Concurrent;
using System.Text.Json;
using System.Web;
using BaseUtils;
using Feed.Trading212._sys._1_Scraping.Structs;
using Feed.Trading212._sys._1_Scraping.Utils;
using LINQPad;
using Microsoft.Playwright;

namespace Feed.Trading212._sys._1_Scraping;


sealed class DataFiller
{
	readonly ConcurrentDictionary<ReportType, ConcurrentDictionary<Quarter, RefField[]>> reports = new();

	
	readonly Lock lockError = new();
	ScrapeError? error;
	void ErrorSet(ScrapeError error_)
	{
		lock (lockError)
			error = error_;
	}
	bool ErrorIsSet()
	{
		bool res;
		lock (lockError)
			res = error != null;
		return res;
	}
	

	static bool isInvalidRequestWritten;


	public Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports => reports.ToDictionary(kv => kv.Key, kv => kv.Value.ToSortedDictionary());


	public void OnRequestFinished(object? _, IRequest req)
	{
		if (ErrorIsSet()) return;

		Task.Run(async () =>
		{
			IResponse? res = null;
			try
			{
				res = await req.ResponseAsync() ?? throw new ArgumentException("Response is null");
				await HandleRequest(req, res);
			}
			catch (Exception ex)
			{
				ErrorSet(ScrapeError.UnexpectedRequestException(ex));
				if (res != null)
				{
					try
					{
						var resTxt = await res.TextAsync();
						await WriteInvalidRequestFileIFN(resTxt);
					}
					catch (Exception ex2)
					{
						ex2.Dump();
					}
				}
			}
		});
	}


	public void ErrorThrowIFN()
	{
		lock (lockError)
		{
			if (error != null)
				throw new ScrapeException(error);
		}
	}


	public HashSet<Quarter> GetReceivedQuarters(ReportType reportType) => reports.TryGetValue(reportType, out var set_) switch
	{
		true => set_.ToHashSet(e => e.Key),
		false => [],
	};




	async Task HandleRequest(IRequest req, IResponse res)
	{
		var reportTypeOpt = Reqs.GetReportType(req);
		if (!reportTypeOpt.HasValue) return;
		var reportType = reportTypeOpt.Value;
		var quarter = Quarter.ParseScrapeQuarter(HttpUtility.ParseQueryString(req.Url)["period"] ?? throw new ArgumentException($"Could not find the period in the url: {req.Url}"));

		var resJson = await ReadJsonAndCheckForRateLimit(res);
		if (!resJson.HasValue) return;
		if (resJson.Value.ValueKind != JsonValueKind.Array) throw new ArgumentException($"Wrong JsonValueKind.  Expected:Array  Actual:{resJson.Value.ValueKind}");

		var jsonArray = resJson.Value
			.Ser()
			.Deser<RefField[]>();

		AddReport(reportType, quarter, jsonArray);
	}


	async Task<JsonElement?> ReadJsonAndCheckForRateLimit(IResponse res)
	{
		try
		{
			var resJson = await res.JsonAsync();
			if (!resJson.HasValue) throw new ArgumentException("!resJson.HasValue");
			return resJson;
		}
		catch (JsonException)
		{

			string? resTxt;
			try
			{
				resTxt = await res.TextAsync();
			}
			catch (Exception ex)
			{
				ex.Dump();
				throw;
			}

			if (resTxt.Contains("You do not have access to trading212.com"))
			{
				ErrorSet(ScrapeError.RateLimit);
				return null;
			}
			else
			{
				throw;
			}
		}
	}




	void AddReport(ReportType reportType, Quarter quarter, RefField[] jsonArray)
	{
		if (!reports.TryGetValue(reportType, out var quarterMap))
			quarterMap = reports[reportType] = new ConcurrentDictionary<Quarter, RefField[]>();
		quarterMap[quarter] = jsonArray;
	}



	async Task WriteInvalidRequestFileIFN(string? resTxt)
	{
		if (isInvalidRequestWritten || resTxt == null) return;
		isInvalidRequestWritten = true;
		await File.WriteAllTextAsync(Consts.Logs.InvalidRequestFile, resTxt);
	}
}
