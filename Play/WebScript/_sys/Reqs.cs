using Microsoft.Playwright;
using WebScript.Structs;

namespace WebScript._sys;

static class Reqs
{
	public static readonly Func<IRequest, bool> Search = req =>
		req.Method == "POST" &&
		req.Url.Contains("algolia");


	public static Func<IRequest, bool> Report(ReportType type) => req =>
		req.Method == "GET" &&
		type switch
		{
			ReportType.FinancialsIncomeStatement => req.Url.Is("financials/income-statement"),
			ReportType.FinancialsBalanceSheet => req.Url.Is("financials/balance-sheet"),
			ReportType.FinancialsCashFlow => req.Url.Is("financials/cash-flow"),

			ReportType.StatsValuation => req.Url.Is("stats/valuation"),
			ReportType.StatsLiquidity => req.Url.Is("stats/liquidity"),
			ReportType.StatsEfficiency => req.Url.Is("stats/efficiency"),
			ReportType.StatsProfitability => req.Url.Is("stats/profitability"),
			ReportType.StatsLeverage => req.Url.Is("stats/leverage"),
			ReportType.StatsPerShare => req.Url.Is("stats/per-share"),
			ReportType.StatsCashFlow => req.Url.Is("stats/cash-flow"),
			ReportType.StatsGrowth => req.Url.Is("stats/growth"),

			_ => throw new ArgumentException($"Unknown ReportType: {type}"),
		};

	public static ReportType? GetReportType(IRequest req)
	{
		if (req.Method != "GET") return null;

		if (req.Url.Is("financials/income-statement")) return ReportType.FinancialsIncomeStatement;
		if (req.Url.Is("financials/balance-sheet")) return ReportType.FinancialsBalanceSheet;
		if (req.Url.Is("financials/cash-flow")) return ReportType.FinancialsCashFlow;

		if (req.Url.Is("stats/valuation")) return ReportType.StatsValuation;
		if (req.Url.Is("stats/liquidity")) return ReportType.StatsLiquidity;
		if (req.Url.Is("stats/efficiency")) return ReportType.StatsEfficiency;
		if (req.Url.Is("stats/profitability")) return ReportType.StatsProfitability;
		if (req.Url.Is("stats/leverage")) return ReportType.StatsLeverage;
		if (req.Url.Is("stats/per-share")) return ReportType.StatsPerShare;
		if (req.Url.Is("stats/cash-flow")) return ReportType.StatsCashFlow;
		if (req.Url.Is("stats/growth")) return ReportType.StatsGrowth;

		return null;
	}



	static bool Is(this string url, string str) => url.StartsWith($"https://live.services.trading212.com/rest/company-details/v2/{str}/quarterly");
}