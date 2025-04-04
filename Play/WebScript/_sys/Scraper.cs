using BaseUtils;
using Microsoft.Playwright;
using System.Text.Json;
using WebScript._sys.Pages;
using WebScript._sys.Utils;
using WebScript.Structs;

namespace WebScript._sys;


/*

TODO: Retry logic
=================

static readonly RetryPolicy RequestRetryPolicy = new(
	3,
	TimeSpan.FromSeconds(60),
	ex => ex is JsonException
);

await page.GetByText("Retry").ClickAsync();

*/
static class Scraper
{
	static readonly RetryPolicy QuarterlyClickRetryPolicy = new(
		20,
		TimeSpan.FromSeconds(0.2),
		ex => ex is TimeoutException
	);


	public static async Task<SymbolData> Scrape(
		SymbolDef symbol,
		HashSet<(ReportType, Quarter)> done
	)
	{
		var dataFiller = new DataFiller();

		/*await Tracing.StartAsync(new TracingStartOptions
		{
			Name = "MyTraceName",
			Screenshots = true,
			Snapshots = true,
			Sources = true,
			Title = "MyTraceTitle",
		});*/

		Page.RequestFinished += dataFiller.OnRequestFinished;

		try
		{
			//Log($"Search {symbol.Item.FullName}");
			await GlobalPage.SearchGoto(symbol);

			foreach (var reportType in Enum.GetValues<ReportType>())
			{
				LogTitle($"{reportType}");
				await SymbolPage.GotoReport(reportType);

				var quarterlyLoc = Page.GetByRole(AriaRole.Button, new() { Name = "Quarterly" }).Last;
				Log($"Click Quarterly: {await quarterlyLoc.IsVisibleAsync()}");
				await Retrier.Retry(
					QuarterlyClickRetryPolicy,
					async () => await quarterlyLoc.ClickAsync(Consts.Click_QuarterlyDelay)
				);

				//Log("Read Quarters");
				var quartersAvailable = await SymbolPage.GetReportQuarters(reportType);
				var quartersTodo = quartersAvailable
					.Where(e => e.Item >= Consts.MinScrapeQuarter)
					//.Where(e => e.Item != firstQuarter)
					.SkipLast(1)
					.Where(e => !done.Contains((reportType, e.Item)))
					.ToArray();
				//Log($"Found {quartersAvailable.Length}. Todo: {quartersTodo.Length}");

				foreach (var quarterTodo in quartersTodo)
				{
					Log($"    Click {quarterTodo.Item}");
					await quarterTodo.Loc.ClickAsync();
				}
			}

			Log("Finished (pause 3s)");
			await Pause(3);

			return dataFiller.Data;
		}
		finally
		{
			Page.RequestFinished -= dataFiller.OnRequestFinished;

			/*await Tracing.StopAsync(new TracingStopOptions
			{
				Path = @"E:\tmp\scrape-traces\trace.zip",
			});*/
		}
	}
}