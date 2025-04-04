using Feed.Trading212._sys.Pages;
using Feed.Trading212._sys.Structs;
using Feed.Trading212._sys.Utils;
using Microsoft.Playwright;

namespace Feed.Trading212._sys;

static class Scraper
{
	public static async Task<ScrapeResult> Scrape(SymbolDef symbol, QuarterSet done)
	{
		var dataFiller = new DataFiller();
		Page.RequestFinished += dataFiller.OnRequestFinished;

		try
		{
			await ScrapeInner(symbol, done);

			return new ScrapeResult(
				symbol,
				null,
				dataFiller.Reports
			);
		}
		catch (Exception ex)
		{
			return new ScrapeResult(
				symbol,
				$"[{ex.GetType().Name}] {ex.Message}",
				dataFiller.Reports
			);
		}
		finally
		{
			Page.RequestFinished -= dataFiller.OnRequestFinished;
		}
	}


	static async Task ScrapeInner(SymbolDef symbol, QuarterSet done)
	{
		var Log = Logger.Make(LogCategory.ScrapeSymbol);

		await GlobalPage.SearchGoto(symbol);

		foreach (var reportType in Enum.GetValues<ReportType>())
		{
			Log.Title($"{reportType}");
			await SymbolPage.GotoReport(reportType);

			var quarterlyLoc = Page.GetByRole(AriaRole.Button, new() { Name = "Quarterly" }).Last;
			Log($"Click Quarterly: {await quarterlyLoc.IsVisibleAsync()}");
			await Retrier.Retry(
				Consts.QuarterlyClickRetryPolicy,
				async () => await quarterlyLoc.ClickAsync(Consts.Click_QuarterlyDelay)
			);

			var quartersAvailable = await SymbolPage.GetReportQuarters(reportType);
			var quartersTodo = quartersAvailable
				.Where(e => e.Item >= Consts.MinScrapeQuarter)
				.SkipLast(1)
				.Where(e => !done.ContainsReportQuarter(reportType, e.Item))
				.ToArray();
			Log($"Found {quartersAvailable.Length}. Todo: {quartersTodo.Length}");


			foreach (var quarterTodo in quartersTodo)
			{
				Log($"    Click {quarterTodo.Item}");
				await quarterTodo.Loc.ClickAsync();
			}
		}

		Log("Finished (pause 3s)");
		await Pause(3);
	}
}