using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace Feed.Trading212._sys._1_Scraping;

static class Locs
{
	public static ILoc SearchBar => page => page.GetByTestId("layout-component").GetByTestId("search-bar");
	public static ILoc SearchButton => page => page.GetByTestId("app-header-search-button");

	public static ILoc SearchResults => page => page.GetByTestId(new Regex("table-search-results")).GetByTestId(new Regex("instrument-search-result-"));
	public static ISubLoc SearchResults_FullName => loc => loc.GetByTestId("instrument-full-name");
	public static ISubLoc SearchResults_ShortName => loc => loc.GetByTestId("short-name");
	public static ISubLoc SearchResults_Exchange => loc => loc.GetByTestId("exchange");

	const string ExpandPrefix = "instrument-screen-section-header";
	public static ILoc ExpandMoreFinancialsFalse => page => page.GetByTestId($"{ExpandPrefix}-keyFinancials").GetByTestId($"{ExpandPrefix}-icon-keyFinancials-false").Last;
	public static ILoc ExpandMoreFinancialsTrue => page => page.GetByTestId($"{ExpandPrefix}-keyFinancials").GetByTestId($"{ExpandPrefix}-icon-keyFinancials-true").Last;
	public static ILoc ExpandMoreFinancialsButton => page => page.GetByTestId("more-financials-button").Last;
	public static ILoc ExpandMoreStatsFalse => page => page.GetByTestId($"{ExpandPrefix}-stats").GetByTestId($"{ExpandPrefix}-icon-stats-false").Last;
	public static ILoc ExpandMoreStatsTrue => page => page.GetByTestId($"{ExpandPrefix}-stats").GetByTestId($"{ExpandPrefix}-icon-stats-true").Last;
	public static ILoc ExpandMoreStatsButton => page => page.GetByTestId("more-stats-button").Last;
	public static ILoc ScreenHeaderBackButton => page => page.GetByTestId("screen-header-back-button").Last;
	public static ILoc ReportButton(ReportType reportType) => reportType switch
	{
		ReportType.FinancialsIncomeStatement => page => page.GetByTestId("income-statement").First,
		ReportType.FinancialsBalanceSheet => page => page.GetByTestId("balance-sheet").First,
		ReportType.FinancialsCashFlow => page => page.GetByTestId("cash-flow").First,

		ReportType.StatsValuation => page => page.GetByTestId("more-stats-screen-section-header-valuation-viewAll").Last,
		ReportType.StatsLiquidity => page => page.GetByTestId("more-stats-screen-section-header-liquidity-viewAll").Last,
		ReportType.StatsEfficiency => page => page.GetByTestId("more-stats-screen-section-header-efficiency-viewAll").Last,
		ReportType.StatsProfitability => page => page.GetByTestId("more-stats-screen-section-header-profitability-viewAll").Last,
		ReportType.StatsLeverage => page => page.GetByTestId("more-stats-screen-section-header-leverage-viewAll").Last,
		ReportType.StatsPerShare => page => page.GetByTestId("more-stats-screen-section-header-per-share-viewAll").Last,
		ReportType.StatsCashFlow => page => page.GetByTestId("more-stats-screen-section-header-cash-flow-viewAll").Last,
		ReportType.StatsGrowth => page => page.GetByTestId("more-stats-screen-section-header-growth-viewAll").Last,

		_ => throw new ArgumentException($"Unknown ReportType: {reportType}"),
	};

	static readonly Regex QuarterRegex = new(@"-tag-component-Q\d'\d\d-text");
	public static ILoc Report_QuarterButton => page => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Quarterly" }).Last;
	public static ILoc Report_Quarters(ReportType reportType) => reportType switch
	{
		ReportType.FinancialsIncomeStatement => page => page.GetByTestId("tab-content-income-statement").GetByTestId(QuarterRegex),
		ReportType.FinancialsBalanceSheet => page => page.GetByTestId("tab-content-balance-sheet").GetByTestId(QuarterRegex),
		ReportType.FinancialsCashFlow => page => page.GetByTestId("tab-content-cash-flow").GetByTestId(QuarterRegex),
		_ => page => page.GetByTestId(QuarterRegex),
	};


	public static ILoc RetryButton => page => page.GetByTestId("retry-button");
	public static ILoc NoConnectionLabel => page => page.GetByTestId("status-bar-no-connection");
}

static class Spots
{
	public const string Search_OpenSearchBar = nameof(Search_OpenSearchBar);
	public const string Search_TypeAndRead = nameof(Search_TypeAndRead);
	public const string Search_ClickResult = nameof(Search_ClickResult);

	public const string GotoReport_MoreFinancialsExpand = nameof(GotoReport_MoreFinancialsExpand);
	public const string GotoReport_MoreFinancialsButton = nameof(GotoReport_MoreFinancialsButton);
	public const string GotoReport_MoreStatsExpand = nameof(GotoReport_MoreStatsExpand);
	public const string GotoReport_MoreStatsButton = nameof(GotoReport_MoreStatsButton);
	public const string GotoReport_ScreenHeaderBackButton = nameof(GotoReport_ScreenHeaderBackButton);
	public const string GotoReport_ReportButton = nameof(GotoReport_ReportButton);
	//public static string GotoReport_ReportButton(ReportType reportType) => $"GotoReport_ReportButton({reportType})";

	public const string Report_QuarterButton = nameof(Report_QuarterButton);
	public const string Report_Quarters = nameof(Report_Quarters);
	//public static string Report_Quarters(ReportType reportType) => $"Report_Quarters({reportType})";
	public const string Report_Quarter = nameof(Report_Quarter);
}