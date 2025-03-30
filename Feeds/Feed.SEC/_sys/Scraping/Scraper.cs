using BaseUtils;
using System.Text.Json.Nodes;
using Feed.SEC._sys.Scraping.Structs;
using Feed.SEC._sys.Utils;
using Microsoft.Playwright;
using static Feed.SEC._sys.Scraping.ScraperConsts;
using System.Text.Json;
using System.Web;
using Feed.SEC._sys.Scraping.Utils;

namespace Feed.SEC._sys.Scraping;





static class Scraper
{
	public static async Task<ScrapeSymbol[]> Search(
		IPage page,
		string text,
		Action<object> log
	)
	{
		var items = await ScraperSearch.Search(page, text, log);
		return items.SelectA(e => e.Item);
	}


	public static async Task Goto(
		IPage page,
		ScrapeSymbol symbol,
		Action<object> log
	)
	{
		var items = await ScraperSearch.Search(page, symbol.FullName, log);
		var loc = items.FirstOrDefault(e => e.Item == symbol)?.Loc;
		if (loc == null)
			throw new ScrapeException("Cannot find symbol in search results");
		await loc.ClickAsync(clickOpt);
	}




	public static async Task<JsonArray> Scrape(
		IPage page,
		ScrapeSymbol symbol,
		Action<object> log
	)
	{
		(Quarter, JsonArray)[] itemsIS = [];
		(Quarter, JsonArray)[] itemsBS = [];
		(Quarter, JsonArray)[] itemsCF = [];

		try
		{

			// Go to the symbol page
			// =====================
			await Goto(page, symbol, log);


			// Click the More Financials button
			// ================================
			await ScraperSymbolPage.ClickMoreFinancialsButton(page);


			var abort = false;

			// Income Statements
			// =================
			if (!abort)
			{
				(itemsIS, var isAbort) = await ExtractCategory(page, "income-statement", log);
				abort |= isAbort;
			}

			// Balance Sheet
			// =============
			if (!abort)
			{
				(itemsBS, var isAbort) = await ExtractCategory(page, "balance-sheet", log);
				abort |= isAbort;
			}

			// Cash Flow
			// =========
			if (!abort)
			{
				(itemsCF, _) = await ExtractCategory(page, "cash-flow", log);
			}

		}
		catch (Exception ex)
		{
			log("Exception");
			log(ex);
		}


		return CompileJson(itemsIS, itemsBS, itemsCF);
	}




	static readonly RetryPolicy RequestRetryPolicy = new(
		3,
		TimeSpan.FromSeconds(60),
		ex => ex is JsonException
	);


	static async Task<((Quarter, JsonArray)[], bool)> ExtractCategory(
		IPage page,
		string category,
		Action<object> log
	)
	{
		var items = new List<(Quarter, JsonArray)>();

		try
		{
			await ScraperSymbolPage.OpenCategory(page, category);

			var firstReq = await ScraperSymbolPage.SelectQuarterlyAndWaitForResponse(page, category);
			var firstItem = await firstReq.Decode();
			items.Add(firstItem);

			var quarters = await ScraperSymbolPage.FindAllQuarters(page, category);
			log($"[{category}] -> {quarters.Length} quarters");

			quarters = quarters.WhereA(e => e.Item != firstItem.Item1);

			foreach (var quarter in quarters)
			{
				var item = await Retrier.Retry(
					RequestRetryPolicy,
					async () =>
					{
						var req = await page.RunAndWaitForRequestFinishedAsync(
							async () => await quarter.Loc.ClickAsync(clickOpt),
							new PageRunAndWaitForRequestFinishedOptions
							{
								Timeout = 10000,
								Predicate = req =>
									req.Method == "GET" &&
									req.Url.StartsWith($"https://live.services.trading212.com/rest/company-details/v2/financials/{category}/quarterly")
							}
						);
						var item = await req.Decode();
						return item;
					},
					async _ =>
					{
						await page.GetByText("Retry").ClickAsync(clickOpt);
					},
					log
				);
				items.Add(item);
			}

			return ([.. items], false);

		}
		catch (Exception ex)
		{
			log($"Exception in {category}");
			log(ex);
			return ([.. items], true);
		}
	}


	static async Task<(Quarter, JsonArray)> Decode(this IRequest req)
	{
		var quarterStr = HttpUtility.ParseQueryString(req.Url)["period"] ?? throw new ArgumentException($"Could not find the period in the url: {req.Url}");
		var quarter = Quarter.ParseScrapeQuarter(quarterStr);
		var arr = await req.ToJsonArray();
		return (quarter, arr);
	}



	static async Task<JsonArray> ToJsonArray(this IRequest req)
	{
		var res = await req.ResponseAsync() ?? throw new ArgumentException("Response is null");
		var resJson = await res.JsonAsync();
		if (!resJson.HasValue) throw new ArgumentException("!resJson.HasValue");
		if (resJson.Value.ValueKind != JsonValueKind.Array) throw new ArgumentException($"Wrong JsonValueKind.  Expected:Array  Actual:{resJson.Value.ValueKind}");
		return JsonUtils.Deser<JsonArray>(JsonUtils.Ser(resJson.Value));
	}

	static JsonArray CompileJson((Quarter, JsonArray)[] itemsIS, (Quarter, JsonArray)[] itemsBS, (Quarter, JsonArray)[] itemsCF) => new([
		CompileJsonCategory("income-statement", itemsIS),
		CompileJsonCategory("balance-sheet", itemsBS),
		CompileJsonCategory("cash-flow", itemsCF),
	]);

	static JsonObject CompileJsonCategory(string name, (Quarter, JsonArray)[] items) => new([
		Mk("name", JsonValue.Create(name)),
		Mk("reports", new JsonArray(items.SelectA(item => CompileJsonCategoryReport(item.Item1, item.Item2)))),
	]);

	static JsonNode CompileJsonCategoryReport(Quarter quarter, JsonArray data) => new JsonObject([
		Mk("quarter", JsonValue.Create(quarter)!),
		Mk("data", data),
	]);

	static KeyValuePair<string, JsonNode> Mk(string key, JsonNode node) => KeyValuePair.Create(key, node);
}