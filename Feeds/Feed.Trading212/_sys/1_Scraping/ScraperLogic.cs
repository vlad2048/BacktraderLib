using BaseUtils;
using Feed.Trading212._sys._1_Scraping.Structs;
using Feed.Trading212._sys._1_Scraping.Utils;
using ScrapeUtils;

namespace Feed.Trading212._sys._1_Scraping;


/*

Rare exceptions
===============
	Spot	: Report_Quarter
	Type	: PlaywrightException
	Message	: Element is outside of the viewport / waiting for GetByTestId(new Regex("-tag-component-Q\\d'\\d\\d-text")).Nth(45)

	Spot	: GotoReport_MoreFinancialsExpand
	Type	: TimeoutException
	Message	: [UnexpectedException] UnexpectedException: [CheckAOrB_ClickBIfB - GotoReport_MoreFinancialsExpand] CheckAOrB failed
*/

static class ScraperLogic
{
	public static async Task Scrape(
		Web web,
		CompanyDef[] companies,
		CancellationToken cancelToken,
		ScrapeOpt opt
	)
	{
		var stateFile = new StateFileHolder(opt.DisableSaving);
		var companiesTodo = companies.WhereA(company => stateFile.NeedsScraping(company, opt.RefreshOldPeriod));
		using var logger = new ScrapeLogger(web.Log, opt, companies.Length, companiesTodo.Length);

		try
		{
			web.CancelToken = cancelToken;
			web.Stats = new FullStatsKeeper();


			if (opt.DryRun)
				return;

			for (var companyIdx = 0; companyIdx < companiesTodo.Length; companyIdx++)
			{
				var company = companiesTodo[companyIdx];

				var stopImmediatly = await ScrapeWithRetries(
					web,
					company,
					companyIdx,
					companiesTodo.Length,
					opt,
					stateFile,
					logger
				);

				if (stopImmediatly)
					break;
			}
		}
		catch (Exception ex)
		{
			logger.LogImpossibleException(ex);
		}
		finally
		{
			web.CancelToken = null;
			web.Stats = null;
			logger.LogFinished();
		}
	}



	static async Task<bool> ScrapeWithRetries(
		Web web,
		CompanyDef company,
		int companyIdx,
		int companyCnt,
		ScrapeOpt opt,
		StateFileHolder stateFile,
		ScrapeLogger logger
	)
	{
		for (var tryIdx = 0; tryIdx < Consts.MaxCompanyScrapeRetryCount; tryIdx++)
		{
			logger.ProgressCompany(company.Name, companyIdx, companyCnt, tryIdx);

			var result = await Scrape(web, company, logger);
			var errorResponse = Consts.GetErrorResponse(result.Error, tryIdx == Consts.MaxCompanyScrapeRetryCount - 1);

			if (!opt.DisableSaving)
			{
				Save(company.Name, result, stateFile, errorResponse is FlagAsErrorErrorResponse);
			}

			logger.LogErrorResponse(errorResponse);
			logger.LogStats(web.Stats!);

			switch (errorResponse)
			{
				case NoneErrorResponse:
					return false;

				case StopImmediatlyErrorResponse:
					return true;

				case FlagAsErrorErrorResponse:
					await web.Reset();
					return false;

				case WaitAndRetryErrorResponse { DelaySeconds: var delay }:
					try
					{
						await web.Sleep(delay);
					}
					catch (Exception ex) when (ex.IsCancel())
					{
						return true;
					}
					await web.Reset();
					break;

				default:
					throw new ArgumentException($"Unknown IErrorResponse: {errorResponse.GetType().Name}");
			}
		}

		return false;
	}



	static async Task<CompanyScrapeResult> Scrape(
		Web web,
		CompanyDef company,
		ScrapeLogger logger
	)
	{
		await web.InitIFN();

		var dataFiller = new DataFiller();
		web.Page.RequestFinished += dataFiller.OnRequestFinished;
		var quartersDone = GetQuartersDone(company.Name);


		try
		{
			await Go(web, company, quartersDone, dataFiller, logger);
			return new CompanyScrapeResult(dataFiller.Reports, null);
		}
		catch (ScrapeException ex)
		{
			return new CompanyScrapeResult(dataFiller.Reports, ex.Error);
		}
		catch (Exception ex) when (ex.IsCancel())
		{
			return new CompanyScrapeResult(dataFiller.Reports, ScrapeError.Cancelled);
		}
		catch (Exception ex) when (!ex.IsCancel())
		{
			return new CompanyScrapeResult(dataFiller.Reports, ScrapeError.UnexpectedException(ex));
		}
		finally
		{
			web.Page.RequestFinished -= dataFiller.OnRequestFinished;
		}
	}



	static async Task Go(
		Web web,
		CompanyDef company,
		QuarterSet quartersDone,
		DataFiller dataFiller,
		ScrapeLogger logger
	)
	{
		await CheckNoConnection(web);

		
		// ************************************
		// * Search and open the company page *
		// ************************************
		await web.Click_B_If_A_NotPresent(Locs.SearchBar, Locs.SearchButton, Spots.Search_OpenSearchBar);

		var items = await web.TypeInSearchBarAndReadResults(
			Locs.SearchBar,
			company.MainTicker,
			Locs.SearchResults,
			async item_ => new LocItem<SearchItem>(
				Locs.SearchResults_FullName(item_),
				new SearchItem(
					await Locs.SearchResults_FullName(item_).InnerTextAsync(),
					await Locs.SearchResults_ShortName(item_).InnerTextAsync(),
					await Locs.SearchResults_Exchange(item_).InnerTextAsync()
				)
			),
			async () =>
			{
				await CheckNoConnection(web);
				throw new TimeoutException($"[TypeInSearchBarAndReadResults - {Spots.Search_TypeAndRead}] Failed to find any items in the search results");
			},
			Spots.Search_TypeAndRead
		);

		var item = items.FirstOrDefault(e => e.Item.ShortName == company.MainTicker && e.Item.Exchange == company.Exchange);
		if (item == null)
			throw new ScrapeException(ScrapeError.CompanyNotFound(company));

		await web.Click(_ => item.Loc, Spots.Search_ClickResult);

		await web.Sleep(1);


		// ***********************************
		// * Go through all the report types *
		// ***********************************
		foreach (var reportType in Enum.GetValues<ReportType>())
		{
			logger.ProgressReport(reportType);
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
					var quarter = quarters[idx];
					logger.ProgressQuarter(quarter, idx, quarters.Length);
					var quarterLoc = quarterMan.GetLoc(quarter);
					if (fast)
						await web.ClickNoCheck(quarterLoc, Spots.Report_Quarter);
					else
						await web.Click(quarterLoc, Spots.Report_Quarter);

					dataFiller.ErrorThrowIFN();
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
				throw new ScrapeException(ScrapeError.QuartersMissing(reportType, remaining));
		}
	}

	/*static async Task CheckRetryButton(Web web)
	{
		var isTrue = await Locs.RetryButton(web.Page).CountAsync() > 0;
		if (isTrue)
			throw new ScrapeException(ScrapeError.NoInternet);
	}*/
	static async Task CheckNoConnection(Web web)
	{
		var isTrue = await Locs.NoConnectionLabel(web.Page).CountAsync() > 0;
		if (isTrue)
			throw new ScrapeException(ScrapeError.NoInternet);
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




	static QuarterSet GetQuartersDone(string companyName)
	{
		var dataFile = Consts.Scraping.CompanyJsonFile(companyName);
		return File.Exists(dataFile) switch
		{
			true => JsonUtils.Load<ScrapeData>(dataFile).Reports.ToDictionary(e => e.Key, e => e.Value.Keys.ToHashSet()),
			false => new QuarterSet(),
		};
	}


	static void Save(string companyName, CompanyScrapeResult result, StateFileHolder stateFile, bool flagAsError)
	{
		var dataFile = Consts.Scraping.CompanyJsonFile(companyName);
		var reportsNext = result.Reports;
		var reportsPrev = File.Exists(dataFile) switch
		{
			true => JsonUtils.Load<ScrapeData>(dataFile).Reports,
			false => new Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>>(),
		};
		new ScrapeData(ReportMerger.Merge(reportsPrev, reportsNext)).Save(dataFile);

		var status = result.Error switch
		{
			null => CompanyScrapeStatus.Success,
			not null => flagAsError switch
			{
				false => CompanyScrapeStatus.InProgress,
				true => CompanyScrapeStatus.Error,
			},
		};
		stateFile.SetState(companyName, status, result.Error?.Message);
	}
}