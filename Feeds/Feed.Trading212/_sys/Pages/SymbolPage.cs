using System.Text.RegularExpressions;
using BaseUtils;
using Feed.Trading212._sys.Structs;

namespace Feed.Trading212._sys.Pages;

static class SymbolPage
{
	static readonly Regex quarterRegex = new(@"-tag-component-Q\d'\d\d-text");

	
	public static async Task GotoReport(ReportType type)
	{
		switch (type)
		{
			case ReportType.FinancialsIncomeStatement:
			{
				var loc = Page.GetByTestId("instrument-screen-section-header-keyFinancials").GetByTestId("instrument-screen-section-header-icon-keyFinancials-true").Last;
				if (await loc.IsVisibleAsync())
					await loc.ClickAsync();
				await Page.GetByTestId("more-financials-button").Last.ClickAsync(Consts.Click_MoreFinancialsDelay);
				await Page.GetByTestId("income-statement").First.ClickAsync();
				break;
			}
			case ReportType.FinancialsBalanceSheet:
				await Page.GetByTestId("balance-sheet").First.ClickAsync();
				break;
			case ReportType.FinancialsCashFlow:
				await Page.GetByTestId("cash-flow").First.ClickAsync();
				break;

			case ReportType.StatsValuation:
			{
				await Page.GetByTestId("screen-header-back-button").Last.ClickAsync();
				var loc = Page.GetByTestId("instrument-screen-section-header-stats").GetByTestId("instrument-screen-section-header-icon-stats-true").Last;
				if (await loc.IsVisibleAsync())
					await loc.ClickAsync();
				await Page.GetByTestId("more-stats-button").Last.ClickAsync();
				await Page.GetByTestId("more-stats-screen-section-header-valuation-viewAll").Last.ClickAsync();
				break;
			}
			case ReportType.StatsLiquidity:
				await Page.GetByTestId("screen-header-back-button").Last.ClickAsync();
				await Page.GetByTestId("more-stats-screen-section-header-liquidity-viewAll").Last.ClickAsync();
				break;
			case ReportType.StatsEfficiency:
				await Page.GetByTestId("screen-header-back-button").Last.ClickAsync();
				await Page.GetByTestId("more-stats-screen-section-header-efficiency-viewAll").Last.ClickAsync();
				break;
			case ReportType.StatsProfitability:
				await Page.GetByTestId("screen-header-back-button").Last.ClickAsync();
				await Page.GetByTestId("more-stats-screen-section-header-profitability-viewAll").Last.ClickAsync();
				break;
			case ReportType.StatsLeverage:
				await Page.GetByTestId("screen-header-back-button").Last.ClickAsync();
				await Page.GetByTestId("more-stats-screen-section-header-leverage-viewAll").Last.ClickAsync();
				break;
			case ReportType.StatsPerShare:
				await Page.GetByTestId("screen-header-back-button").Last.ClickAsync();
				await Page.GetByTestId("more-stats-screen-section-header-per-share-viewAll").Last.ClickAsync();
				break;
			case ReportType.StatsCashFlow:
				await Page.GetByTestId("screen-header-back-button").Last.ClickAsync();
				await Page.GetByTestId("more-stats-screen-section-header-cash-flow-viewAll").Last.ClickAsync();
				break;
			case ReportType.StatsGrowth:
				await Page.GetByTestId("screen-header-back-button").Last.ClickAsync();
				await Page.GetByTestId("more-stats-screen-section-header-growth-viewAll").Last.ClickAsync();
				break;

			default:
				throw new ArgumentException($"Unknown ReportType: {type}");
		}
	}


	


	public static async Task<LocItem<Quarter>[]> GetReportQuarters(ReportType type)
	{
		var loc = type switch
		{
			ReportType.FinancialsIncomeStatement => Page.GetByTestId("tab-content-income-statement").GetByTestId(quarterRegex),
			ReportType.FinancialsBalanceSheet => Page.GetByTestId("tab-content-balance-sheet").GetByTestId(quarterRegex),
			ReportType.FinancialsCashFlow => Page.GetByTestId("tab-content-cash-flow").GetByTestId(quarterRegex),
			_ => Page.GetByTestId(quarterRegex),
		};
		var items = await loc.AllAsync();
		var list = new List<LocItem<Quarter>>();
		foreach (var item in items)
		{
			var attr = await item.GetAttributeAsync("data-testid") ?? throw new ArgumentException("Expected a data-testid here");
			list.Add(new LocItem<Quarter>(Quarter.ParseScrapeQuarter(attr), item));
		}
		return [.. list];
	}
}


