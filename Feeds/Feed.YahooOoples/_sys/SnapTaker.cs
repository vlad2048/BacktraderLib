using OoplesFinance.YahooFinanceAPI;
using OoplesFinance.YahooFinanceAPI.Enums;

namespace Feed.YahooOoples._sys;

static class SnapTaker
{
	const Country country = Country.UnitedStates;
	const int count = 10;


	public static bool SaveTodaySnap(YahooClient client)
	{
		var date = DateOnly.FromDateTime(DateTime.Now);
		var file = Consts.Snap.GetDateSnapFile(date);
		if (File.Exists(file))
			return false;

		var snap = client.Snap();

		snap.Save(file);
		return true;
	}


	public static Snap LoadSnap(YahooClient client, DateOnly? date)
	{
		date ??= DateOnly.FromDateTime(DateTime.Now);
		var dateV = date.Value;
		var file = Consts.Snap.GetDateSnapFile(dateV);
		Snap snap;
		if (!File.Exists(file))
		{
			snap = client.Snap();
			snap.Save(file);
		}
		else
		{
			snap = JsonUtils.Load<Snap>(file);
		}
		return snap;
	}



	static Snap Snap(this YahooClient client) => new(
		[..client.GetMarketSummaryAsync().Result],

		[..client.GetTopTrendingStocksAsync(country, count).Result],

		client.GetAnalystStrongBuyStocksAsync(count).Result,
		client.GetLatestAnalystUpgradedStocksAsync(count).Result,

		client.GetTopBearishStocksRightNowAsync(count).Result,
		client.GetTopBullishStocksRightNowAsync(count).Result,
		client.GetTopUpsideBreakoutStocksAsync(count).Result,

		client.GetTopStocksOwnedByCathieWoodAsync(count).Result,
		client.GetTopStocksOwnedByGoldmanSachsAsync(count).Result,
		client.GetTopStocksOwnedByWarrenBuffetAsync(count).Result,
		client.GetTopStocksOwnedByRayDalioAsync(count).Result,

		client.GetMostInstitutionallyBoughtLargeCapStocksAsync(count).Result,
		client.GetMostInstitutionallyHeldLargeCapStocksAsync(count).Result,
		client.GetMostInstitutionallySoldLargeCapStocksAsync(count).Result,
		client.GetStocksWithMostInstitutionalBuyersAsync(count).Result,
		client.GetStocksWithMostInstitutionalSellersAsync(count).Result,
		client.GetStocksMostBoughtByHedgeFundsAsync(count).Result,
		client.GetStocksMostBoughtByPensionFundsAsync(count).Result,
		client.GetStocksMostBoughtByPrivateEquityAsync(count).Result,
		client.GetStocksMostBoughtBySovereignWealthFundsAsync(count).Result,

		client.GetTopGainersAsync(count).Result,
		client.GetTopLosersAsync(count).Result,
		client.GetSmallCapGainersAsync(count).Result,
		client.GetMostActiveStocksAsync(count).Result,
		client.GetAggressiveSmallCapStocksAsync(count).Result,
		client.GetConservativeForeignFundsAsync(count).Result,
		client.GetGrowthTechnologyStocksAsync(count).Result,
		client.GetHighYieldBondsAsync(count).Result,
		client.GetMostShortedStocksAsync(count).Result,
		client.GetPortfolioAnchorsAsync(count).Result,
		client.GetSolidLargeGrowthFundsAsync(count).Result,
		client.GetSolidMidcapGrowthFundsAsync(count).Result,
		client.GetTopMutualFundsAsync(count).Result,
		client.GetUndervaluedGrowthStocksAsync(count).Result,
		client.GetUndervaluedLargeCapStocksAsync(count).Result,
		client.GetUndervaluedWideMoatStocksAsync(count).Result,
		client.GetMorningstarFiveStarStocksAsync(count).Result,
		client.GetStrongUndervaluedStocksAsync(count).Result
	);
}