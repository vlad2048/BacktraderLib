using BaseUtils;
using Microsoft.Playwright;
using System.Text.RegularExpressions;
using WebScript._sys;
using WebScript._sys.Pages;
using WebScript._sys.Structs;
using WebScript._sys.Utils;
using WebScript.Structs;

namespace WebScript;


static class Symbols
{
	public static readonly SymbolDef Tesla = new(new SearchItem("Tesla", "TSLA", "NASDAQ"), "TESLA, INC.");
	public static readonly SymbolDef Macy = new(new SearchItem("Macy's", "M", "NYSE"), "MACY'S, INC.");
}


static class Script
{
	public static async Task Main(IPage page, ITracing tracing, Action<object> log)
	{
		(Page, Tracing, Globals.log) = (page, tracing, log);

		Log("v14");

		//Log((await Page.GetDataTestIdTree()).FilterUpto(e => e.Contains("search")));
		//await GlobalPage.SearchGoto(Symbols.Tesla);

		await RunScrape();
		//await Page.GetByRole(AriaRole.Button, new() { Name = "Quarterly" }).Last.ClickAsync(Consts.Click_QuarterlyDelay);

		//div[@data-testid='layout-component']//div[@data-testid='search-bar')]

		//Log($"cnt:{await Page.GetByTestId("instrument-screen-section-header-stats").GetByTestId("instrument-screen-section-header-icon-stats-true").CountAsync()}");
		//Log($"vis:{await Page.GetByTestId("instrument-screen-section-header-stats").GetByTestId("instrument-screen-section-header-icon-stats-true").Last.IsVisibleAsync()}");

		Log("done");
	}


	static async Task RunScrape()
	{
		var done = new HashSet<(ReportType, Quarter)>();
		var file = @"C:\ProgramData\Feed.Trading212\data.json";
		var data = await Scraper.Scrape(Symbols.Tesla, done);
		data.Save(file);
		Log($"Done: {data.Reports.Sum(kv => kv.Value.Quarters.Count)}");
	}
}