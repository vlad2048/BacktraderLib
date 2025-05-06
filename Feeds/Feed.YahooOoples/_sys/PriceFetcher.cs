using BaseUtils;
using OoplesFinance.YahooFinanceAPI;
using OoplesFinance.YahooFinanceAPI.Enums;
using OoplesFinance.YahooFinanceAPI.Models;

namespace Feed.YahooOoples._sys;



static class PriceFetcher
{
	public static OoplesPrice FetchPrice(this YahooClient client, string symbol)
	{
		// @formatter:off
		var data = client.GetAllHistoricalDataAsync(
			symbol:					symbol,
			dataFrequency:			DataFrequency.Daily,
			startDate:				Consts.TimeStart,//.AddTicks(1),
			endDate:				DateTime.Now.Date,
			includeAdjustedClose:	true
		).Result;
		// @formatter:on

		return new OoplesPrice(
			symbol,
			data.ToBars(),
			data.Dividends.SelectA(e => new OoplesDividend(
				e.Date,
				e.Amount ?? throw new ArgumentException("Dividend.Amount is null")
			)),
			data.Splits.SelectA(e => new OoplesSplit(
				e.Date ?? throw new ArgumentException("Split.Date is null"),
				e.Numerator ?? throw new ArgumentException("Split.Numerator is null"), e.Denominator ?? throw new ArgumentException("Split.Denominator is null")
			))
		);
	}


	static OoplesBar[] ToBars(this HistoricalFullData data) =>
		data.Prices.SelectA(e => new OoplesBar(
			e.Date,
			e.Open,
			e.High,
			e.Low,
			e.Close,
			e.AdjustedClose,
			e.Volume
		));
}

/*
using BaseUtils;
using Feed.YahooOoples._sys.Errors;
using Feed.YahooOoples._sys.Structs;
using OoplesFinance.YahooFinanceAPI;
using OoplesFinance.YahooFinanceAPI.Enums;
using OoplesFinance.YahooFinanceAPI.Models;

namespace Feed.YahooOoples._sys;



static class PriceFetcher
{
	public static OoplesPrice FetchPrice(this YahooClient client, string symbol) =>
		Consts.FetchStrat switch
		{
			FetchStrat.None => client.FetchPrice_None(symbol),
			FetchStrat.UnapplySplits => client.FetchPrice_UnapplySplits(symbol),
			FetchStrat.CompareCloseAndCloseAdj => client.FetchPrice_CompareCloseAndCloseAdj(symbol),
			_ => throw new NotImplementedException(),
		};



	static OoplesPrice FetchPrice_None(this YahooClient client, string symbol)
	{
		var data = client.GetAllHistoricalDataAsync(symbol, DataFrequency.Daily, Consts.TimeStart, DateTime.Now.Date, false).Result;
		var price = new OoplesPrice(
			symbol,
			data.ToBars_UnapplySplits(),
			data.Dividends.SelectA(e => new OoplesDividend(e.Date, e.Amount ?? throw new ArgumentException("Dividend.Amount is null"))),
			data.Splits.SelectA(e => new OoplesSplit(e.Date ?? throw new ArgumentException("Split.Date is null"), e.Numerator ?? throw new ArgumentException("Split.Numerator is null"), e.Denominator ?? throw new ArgumentException("Split.Denominator is null")))
		);
		return price.UnapplySplits();
	}




	sealed record OoplesPriceFile(
		DateTime QueryTMax,
		OoplesPrice Data
	);




	// ********************************************
	// ********************************************
	// **** FetchStrat.CompareCloseAndCloseAdj ****
	// ********************************************
	// ********************************************
	static OoplesPrice FetchPrice_CompareCloseAndCloseAdj(this YahooClient client, string symbol)
	{
		try
		{
			var file = Consts.Data.GetSymbolFile(symbol);
			var tMax = DateTime.Now.SnapToMonth();
			if (!File.Exists(file))
			{
				var data = client.Get_CompareCloseAndCloseAdj(symbol, tMax).SanityCheck();
				new OoplesPriceFile(tMax, data).Save(file);
				return data;
			}
			else
			{
				var dataFilePrev = JsonUtils.Load<OoplesPriceFile>(file);
				var delay = tMax - dataFilePrev.QueryTMax;
				if (delay >= Consts.FetchDelay)
				{
					var dataNext = client.Get_UnapplySplits(symbol, dataFilePrev.QueryTMax, tMax).SanityCheck();
					if (dataNext.Bars.Length > 0)
					{
						new OoplesPriceFile(tMax, dataNext).Save(file);
						return dataNext;
					}
					else
					{
						return dataFilePrev.Data;
					}
				}
				else
				{
					return dataFilePrev.Data;
				}
			}
		}
		catch (Exception ex)
		{
			throw new YahooOoplesException(symbol, ex);
		}
	}

	static OoplesPrice Get_CompareCloseAndCloseAdj(this YahooClient client, string symbol, DateTime tMax)
	{
		var data = client.GetAllHistoricalDataAsync(symbol, DataFrequency.Daily, Consts.TimeStart, tMax, true).Result;
		var price = new OoplesPrice(
			symbol,
			data.ToBars_CompareCloseAndCloseAdj(),
			data.Dividends.SelectA(e => new OoplesDividend(e.Date, e.Amount ?? throw new ArgumentException("Dividend.Amount is null"))),
			data.Splits.SelectA(e => new OoplesSplit(e.Date ?? throw new ArgumentException("Split.Date is null"), e.Numerator ?? throw new ArgumentException("Split.Numerator is null"), e.Denominator ?? throw new ArgumentException("Split.Denominator is null")))
		);
		return price;
	}


	sealed record OoplesBarWithAdj(
		DateTime Date,
		double Open,
		double High,
		double Low,
		double Close,
		double CloseAdj,
		double Volume
	);

	static OoplesBar[] ToBars_CompareCloseAndCloseAdj(this HistoricalFullData data)
	{
		var barsAdj = data.Prices.SelectA(e => new OoplesBarWithAdj(
			e.Date,
			e.Open,
			e.High,
			e.Low,
			e.Close,
			e.AdjustedClose,
			e.Volume
		));
		var barsDst = new OoplesBar[barsAdj.Length];
		for (var i = 0; i < barsAdj.Length; i++)
		{
			var barAdj = barsAdj[i];
			var delta = barAdj.CloseAdj - barAdj.Close;

			barsDst[i] = new OoplesBar(
				barAdj.Date,
				barAdj.Open + delta,
				barAdj.High + delta,
				barAdj.Low + delta,
				barAdj.CloseAdj,
				barAdj.Volume
			);
		}

		return barsDst;
	}


	static DateTime SnapToMonth(this DateTime e) => new DateTime(e.Year, e.Month, 1).AddTicks(-1);





	// **********************************
	// **********************************
	// **** FetchStrat.UnapplySplits ****
	// **********************************
	// **********************************
	static OoplesPrice FetchPrice_UnapplySplits(this YahooClient client, string symbol)
	{
		try
		{
			var file = Consts.Data.GetSymbolFile(symbol);
			var tMax = DateTime.Now.Date.Date;
			if (!File.Exists(file))
			{
				var data = client.Get_UnapplySplits(symbol, Consts.TimeStart, tMax).SanityCheck();
				new OoplesPriceFile(tMax, data).Save(file);
				return data.Adjust();
			}
			else
			{
				var dataFilePrev = JsonUtils.Load<OoplesPriceFile>(file);
				var delay = tMax - dataFilePrev.QueryTMax;
				if (delay >= Consts.FetchDelay)
				{
					var dataNext = client.Get_UnapplySplits(symbol, dataFilePrev.QueryTMax, tMax).SanityCheck();
					if (dataNext.Bars.Length > 0)
					{
						var dataMerge = Merge(dataFilePrev.Data, dataNext).SanityCheck();
						new OoplesPriceFile(tMax, dataMerge).Save(file);
						return dataMerge.Adjust();
					}
					else
					{
						return dataFilePrev.Data.Adjust();
					}
				}
				else
				{
					return dataFilePrev.Data.Adjust();
				}
			}
		}
		catch (Exception ex)
		{
			throw new YahooOoplesException(symbol, ex);
		}
	}


	static OoplesPrice Get_UnapplySplits(this YahooClient client, string symbol, DateTime tMinStrict, DateTime tMax)
	{
		var data = client.GetAllHistoricalDataAsync(symbol, DataFrequency.Daily, tMinStrict.AddTicks(1), tMax, false).Result;
		var price = new OoplesPrice(
			symbol,
			data.ToBars_UnapplySplits(),
			data.Dividends.SelectA(e => new OoplesDividend(e.Date, e.Amount ?? throw new ArgumentException("Dividend.Amount is null"))),
			data.Splits.SelectA(e => new OoplesSplit(e.Date ?? throw new ArgumentException("Split.Date is null"), e.Numerator ?? throw new ArgumentException("Split.Numerator is null"), e.Denominator ?? throw new ArgumentException("Split.Denominator is null")))
		);
		return price.UnapplySplits();
	}



	static OoplesPrice Merge(OoplesPrice dataPrev, OoplesPrice dataNext) => new(
		dataPrev.Symbol,
		dataPrev.Bars.Concat(dataNext.Bars).ToArray(),
		dataPrev.Dividends.Concat(dataNext.Dividends).ToArray(),
		dataPrev.Splits.Concat(dataNext.Splits).ToArray()
	);

	static OoplesPrice SanityCheck(this OoplesPrice price)
	{
		var n = price.Bars.Length;
		var dates = price.Bars.SelectA(e => e.Date.Date).Distinct().ToArray();
		var cntDivs = price.Dividends.Distinct().Count();
		var cntSpls = price.Splits.Distinct().Count();

		if (dates.Length != n) throw new ArgumentException("Non unique dates");
		if (cntDivs != price.Dividends.Length) throw new ArgumentException("Non unique dividends");
		if (cntSpls != price.Splits.Length) throw new ArgumentException("Non unique splits");

		return price;
	}



	static OoplesBar[] ToBars_UnapplySplits(this HistoricalFullData data) =>
		data.Prices.SelectA(e => new OoplesBar(
			e.Date,
			e.Open,
			e.High,
			e.Low,
			e.Close,
			e.Volume
		));

	static OoplesPrice UnapplySplits(this OoplesPrice priceSrc)
	{
		var barsDst = priceSrc.Bars.ToArray();

		foreach (var split in Enumerable.Reverse(priceSrc.Splits))
		{
			var splitRatio = split.ToFactor / split.ForFactor;
			for (var i = 0; i < barsDst.Length; i++)
			{
				if (barsDst[i].Date < split.ExDate)
				{
					barsDst[i] = barsDst[i] with
					{
						Open = barsDst[i].Open * splitRatio,
						High = barsDst[i].High * splitRatio,
						Low = barsDst[i].Low * splitRatio,
						Close = barsDst[i].Close * splitRatio,
						Volume = (long)(barsDst[i].Volume / splitRatio),
					};
				}
			}
		}

		return priceSrc with { Bars = barsDst };
	}


	static OoplesPrice Adjust(this OoplesPrice priceSrc)
	{
		var barsDst = priceSrc.Bars.ToArray();

		foreach (var dividend in Enumerable.Reverse(priceSrc.Dividends))
			for (var i = 0; i < barsDst.Length; i++)
			{
				if (barsDst[i].Date < dividend.ExDate)
				{
					barsDst[i] = barsDst[i] with
					{
						Open = barsDst[i].Open - dividend.Amount,
						High = barsDst[i].High - dividend.Amount,
						Low = barsDst[i].Low - dividend.Amount,
						Close = barsDst[i].Close - dividend.Amount,
					};
				}
			}

		foreach (var split in Enumerable.Reverse(priceSrc.Splits))
		{
			var splitRatio = split.ToFactor / split.ForFactor;
			for (var i = 0; i < barsDst.Length; i++)
			{
				if (barsDst[i].Date < split.ExDate)
				{
					barsDst[i] = barsDst[i] with
					{
						Open = barsDst[i].Open / splitRatio,
						High = barsDst[i].High / splitRatio,
						Low = barsDst[i].Low / splitRatio,
						Close = barsDst[i].Close / splitRatio,
						Volume = (long)(barsDst[i].Volume * splitRatio),
					};
				}
			}
		}

		return priceSrc with { Bars = barsDst };
	}
}
*/