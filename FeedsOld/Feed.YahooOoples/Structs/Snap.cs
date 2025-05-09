using OoplesFinance.YahooFinanceAPI.Models;

namespace Feed.YahooOoples;

public sealed record Snap(
	MarketSummaryResult[] MarketSummary,

	string[] TopTrendingStocks,

	AnalystResult AnalystStrongBuyStocks,
	AnalystResult LatestAnalystUpgradedStocks,

	TrendingStocksResult TopBearishStocksRightNow,
	TrendingStocksResult TopBullishStocksRightNow,
	TrendingStocksResult TopUpsideBreakoutStocks,

	StocksOwnedResult TopStocksOwnedByCathieWood,
	StocksOwnedResult TopStocksOwnedByGoldmanSachs,
	StocksOwnedResult TopStocksOwnedByWarrenBuffet,
	StocksOwnedResult TopStocksOwnedByRayDalio,

	InstitutionResult MostInstitutionallyBoughtLargeCapStocks,
	InstitutionResult MostInstitutionallyHeldLargeCapStocks,
	InstitutionResult MostInstitutionallySoldLargeCapStocks,
	InstitutionResult StocksWithMostInstitutionalBuyers,
	InstitutionResult StocksWithMostInstitutionalSellers,
	InstitutionResult StocksMostBoughtByHedgeFunds,
	InstitutionResult StocksMostBoughtByPensionFunds,
	InstitutionResult StocksMostBoughtByPrivateEquity,
	InstitutionResult StocksMostBoughtBySovereignWealthFunds,

	ScreenerResult TopGainers,
	ScreenerResult TopLosers,
	ScreenerResult SmallCapGainers,
	ScreenerResult MostActiveStocks,
	ScreenerResult AggressiveSmallCapStocks,
	ScreenerResult ConservativeForeignFunds,
	ScreenerResult GrowthTechnologyStocks,
	ScreenerResult HighYieldBonds,
	ScreenerResult MostShortedStocks,
	ScreenerResult PortfolioAnchors,
	ScreenerResult SolidLargeGrowthFunds,
	ScreenerResult SolidMidcapGrowthFunds,
	ScreenerResult TopMutualFunds,
	ScreenerResult UndervaluedGrowthStocks,
	ScreenerResult UndervaluedLargeCapStocks,
	ScreenerResult UndervaluedWideMoatStocks,
	ScreenerResult MorningstarFiveStarStocks,
	ScreenerResult StrongUndervaluedStocks
);