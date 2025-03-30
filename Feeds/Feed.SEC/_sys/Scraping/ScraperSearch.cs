using Feed.SEC._sys.Scraping.Structs;
using Microsoft.Playwright;
using System.Text.RegularExpressions;
using static Feed.SEC._sys.Scraping.ScraperConsts;

namespace Feed.SEC._sys.Scraping;


static class ScraperSearch
{
	public static async Task<LocItem<ScrapeSymbol>[]> Search(IPage page, string text, Action<object> log)
	{
		if (await page.GetByTestId("search-bar").CountAsync() == 0)
		{
			await page.GetByTestId("app-header-search-button").ClickAsync(clickOpt);
		}

		await page.RunAndWaitForRequestFinishedAsync(
			async () => await page.GetByTestId("search-bar").FillAsync(text, fillOpt),
			new PageRunAndWaitForRequestFinishedOptions
			{
				Timeout = 10000,
				Predicate = req =>
					req.Method == "POST" &&
					req.Url.Contains("algolia")
			}
		);

		var items = await ReadResults(page, log);
		return items;
	}


	/*static async Task ClickButton(IPage page)
	{
		await page.GetByTestId("app-header-search-button").ClickAsync(clickOpt);
	}

	static async Task TypeAndWait(IPage page, string text)
	{
		await page.RunAndWaitForRequestFinishedAsync(
			async () => await page.GetByTestId("search-bar").FillAsync(text, fillOpt),
			new PageRunAndWaitForRequestFinishedOptions
			{
				Timeout = 10000,
				Predicate = req =>
					req.Method == "POST" &&
					req.Url.Contains("algolia")
			}
		);
	}*/

	static async Task<LocItem<ScrapeSymbol>[]> ReadResults(IPage page, Action<object> log)
	{
		var items = await page.GetByTestId(new Regex("table-search-results")).GetByTestId(new Regex("instrument-search-result-")).AllAsync();
		var list = new List<LocItem<ScrapeSymbol>>();
		foreach (var item in items)
		{
			var fullNameLoc = item.GetByTestId("instrument-full-name");
			var fullName = await fullNameLoc.InnerTextAsync(locInnerTextOpt);
			var shortName = await item.GetByTestId("short-name").InnerTextAsync(locInnerTextOpt);
			var exchange = await item.GetByTestId("exchange").InnerTextAsync(locInnerTextOpt);
			list.Add(new LocItem<ScrapeSymbol>(new ScrapeSymbol(fullName, shortName, exchange), fullNameLoc));
		}
		return [.. list];
	}
}