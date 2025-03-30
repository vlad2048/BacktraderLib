using BaseUtils;
using Feed.SEC._sys.Scraping.Structs;
using Microsoft.Playwright;
using System.Text.RegularExpressions;
using static Feed.SEC._sys.Scraping.ScraperConsts;

namespace Feed.SEC._sys.Scraping;


static class ScraperSymbolPage
{
	public static async Task ClickMoreFinancialsButton(IPage page)
	{
		await page.GetByTestId("more-financials-button").ClickAsync(clickOpt);
	}

	public static async Task OpenCategory(IPage page, string category)
	{
		await page.GetByTestId(category).First.ClickAsync(clickOpt);
	}

	public static async Task SelectQuarterly(IPage page, string category)
	{
		await page.GetByTestId($"tab-content-{category}").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Quarterly" }).ClickAsync(clickOpt);
	}

	public static async Task<IRequest> SelectQuarterlyAndWaitForResponse(IPage page, string category) =>
		await page.RunAndWaitForRequestFinishedAsync(
			async () => await SelectQuarterly(page, category),
			new PageRunAndWaitForRequestFinishedOptions
			{
				Timeout = 10000,
				Predicate = req =>
					req.Method == "GET" &&
					req.Url.StartsWith($"https://live.services.trading212.com/rest/company-details/v2/financials/{category}/quarterly")
			}
		);


	public static async Task<LocItem<Quarter>[]> FindAllQuarters(IPage page, string category)
	{
		var items = await page.GetByTestId($"tab-content-{category}").GetByTestId(new Regex(@"-tag-component-Q\d'\d\d-text")).AllAsync();
		var list = new List<LocItem<Quarter>>();
		foreach (var item in items)
		{
			var attr = (await item.GetAttributeAsync("data-testid")) ?? throw new ArgumentException("Expected a data-testid here");
			list.Add(new LocItem<Quarter>(Quarter.ParseScrapeQuarter(attr), item));
		}
		return [.. list];
	}
}