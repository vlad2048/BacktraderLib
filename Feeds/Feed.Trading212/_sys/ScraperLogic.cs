using BaseUtils;
using Feed.Trading212._sys.Structs;
using ScrapeUtils;

namespace Feed.Trading212._sys;

static class ScraperLogic
{
	public static async Task<IScrapeResult> Scrape(Web web, CompanySearchInfo companySearchInfo, ScrapeOpt opt)
	{
		if (!ScrapeSaver.IsScrapeNeeded(companySearchInfo.SecCompanyName, out var quartersDone))
		{
			web.Log.AppendContent($"No scraping needed to '{companySearchInfo.SecCompanyName}'");
			return ScrapeResult.NoScrapeNeeded;
		}
		await web.InitIFN();



		var scrapeLogger = new ScrapeLogger(web.Log);
		var dataFiller = new DataFiller(opt);
		web.OnStatsChanged += scrapeLogger.LogStats;
		web.Page.RequestFinished += dataFiller.OnRequestFinished;


		try
		{
			await Go(web, companySearchInfo, quartersDone, dataFiller, scrapeLogger);
			scrapeLogger.LogFinished();

			return ScrapeResult.Success(dataFiller.GetReports());
		}
		catch (ScrapeException ex) when (ex.ErrorType is ScrapeErrorType.RateLimit)
		{
			return ScrapeResult.RateLimit(dataFiller.GetReports());
		}
		catch (ScrapeException ex) when (ex.ErrorType is ScrapeErrorType.NoInternet)
		{
			return ScrapeResult.NoInternet(dataFiller.GetReports());
		}
		catch (Exception ex)
		{
			return ScrapeResult.Error(ex, dataFiller.GetReports());
		}
		finally
		{
			web.Page.RequestFinished -= dataFiller.OnRequestFinished;
			web.OnStatsChanged -= scrapeLogger.LogStats;
		}
	}



	static async Task Go(
		Web web,
		CompanySearchInfo companySearchInfo,
		QuarterSet quartersDone,
		DataFiller dataFiller,
		ScrapeLogger scrapeLogger
	)
	{
		await CheckNoConnection(web);

		
		// ************************************
		// * Search and open the company page *
		// ************************************
		await web.Click_B_If_A_NotPresent(Locs.SearchBar, Locs.SearchButton, Spots.Search_OpenSearchBar);

		var items = await web.TypeInSearchBarAndReadResults(
			Locs.SearchBar,
			companySearchInfo.Ticker,
			Locs.SearchResults,
			async item_ => new LocItem<SearchItem>(
				Locs.SearchResults_FullName(item_),
				new SearchItem(
					await Locs.SearchResults_FullName(item_).InnerTextAsync(),
					await Locs.SearchResults_ShortName(item_).InnerTextAsync(),
					await Locs.SearchResults_Exchange(item_).InnerTextAsync()
				)
			),
			Spots.Search_TypeAndRead
		);

		var item = items.FirstOrDefault(e => e.Item.ShortName == companySearchInfo.Ticker && e.Item.Exchange == companySearchInfo.Exchange);
		if (item == null)
			throw ScrapeException.SymbolNotFound(companySearchInfo);

		await web.Click(_ => item.Loc, Spots.Search_ClickResult);

		await web.Sleep(1);


		// ***********************************
		// * Go through all the report types *
		// ***********************************
		foreach (var reportType in Enum.GetValues<ReportType>())
		{
			scrapeLogger.LogReportProgress(reportType);
			await GotoReport(web, reportType);

			await web.Sleep(1);
			await web.Click(Locs.Report_QuarterButton, Spots.Report_QuarterButton);
			var quartersPage = (await web.ReadItems(
				Locs.Report_Quarters(reportType),
				async item_ => new LocItem<Quarter>(
					item_,
					Quarter.ParseScrapeQuarter(
						await item_.GetAttributeAsync("data-testid") ?? throw new ArgumentException("Expected a data-testid here")
					)
				),
				Spots.Report_Quarters
			))
				.Where(e => e.Item >= Consts.MinScrapeQuarter)
				.OrderBy(e => e.Item)
				.ToArray();

			var quarterMan = new QuarterMan(
				dataFiller,
				reportType,
				quartersPage,
				quartersDone
			);

			async Task Do(Quarter[] quarters, bool fast)
			{
				for (var idx = 0; idx < quarters.Length; idx++)
				{
					scrapeLogger.LogQuarterProgress(idx, quarters.Length);
					var quarter = quarters[idx];
					var quarterLoc = quarterMan.GetLoc(quarter);
					if (fast)
						await web.ClickNoCheck(quarterLoc, Spots.Report_Quarter);
					else
						await web.Click(quarterLoc, Spots.Report_Quarter);

					dataFiller.ExceptionThrowIFN();
				}
			}


			foreach (var loop in Consts.QuarterLoops)
			{
				var quartersTodo = quarterMan.GetQuartersToClick();
				await Do(quartersTodo, loop);

				if (quarterMan.GetQuartersToClick().Length > 0)
				{
					//await CheckRetryButton(web);
					await CheckNoConnection(web);
					await web.Sleep(1);
				}
			}

			//scrapeLogger.LogRemaining(reportType, quarterMan.GetQuartersToClick());

			var remaining = quarterMan.GetQuartersToClick();
			if (remaining.Length > 0)
				throw ScrapeException.QuartersMissing(reportType, remaining);
		}
	}

	static async Task CheckRetryButton(Web web)
	{
		var isTrue = await Locs.RetryButton(web.Page).CountAsync() > 0;
		if (isTrue)
			throw ScrapeException.NoInternet;
	}
	static async Task CheckNoConnection(Web web)
	{
		var isTrue = await Locs.NoConnectionLabel(web.Page).CountAsync() > 0;
		if (isTrue)
			throw ScrapeException.NoInternet;
	}


	static async Task GotoReport(Web web, ReportType reportType)
	{
		if (reportType is >= ReportType.StatsValuation and <= ReportType.StatsGrowth)
		{
			await web.Click(Locs.ScreenHeaderBackButton, Spots.GotoReport_ScreenHeaderBackButton);
		}

		// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
		switch (reportType)
		{
			case ReportType.FinancialsIncomeStatement:
				await web.CheckAOrB_ClickBIfB(Locs.ExpandMoreFinancialsFalse, Locs.ExpandMoreFinancialsTrue, Spots.GotoReport_MoreFinancialsExpand);
				await web.Click(Locs.ExpandMoreFinancialsButton, Spots.GotoReport_MoreFinancialsButton);
				break;

			case ReportType.StatsValuation:
				await web.CheckAOrB_ClickBIfB(Locs.ExpandMoreStatsFalse, Locs.ExpandMoreStatsTrue, Spots.GotoReport_MoreStatsExpand);
				await web.Click(Locs.ExpandMoreStatsButton, Spots.GotoReport_MoreStatsButton);
				break;
		}

		await web.Click(Locs.ReportButton(reportType), Spots.GotoReport_ReportButton);
	}
}