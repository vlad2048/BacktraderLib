using Feed.Trading212;
using Frames;

namespace Feed.Best;

public sealed record Dividend(DateOnly ExDate, double Amount);

public sealed record Split(DateOnly ExDate, double ToFactor, double ForFactor);

/// <summary>
/// Record representing a financial instrument.
/// </summary>
/// <param name="Id">FIGI code. See <see href="https://en.wikipedia.org/wiki/Financial_Instrument_Global_Identifier">wikipedia</see></param>
/// <param name="Ticker">Ticker</param>
/// <param name="Company">Company name as per SEC filing</param>
/// <param name="Country">Country</param>
/// <param name="Currency">Currency</param>
/// <param name="Exchange">Exchange</param>
/// <param name="MicCode">Exchange code. See <see href="https://en.wikipedia.org/wiki/Market_Identifier_Code">wikipedia</see></param>
/// <param name="CfiCode">Type of instrument. See <see href="https://en.wikipedia.org/wiki/ISO_10962">wikipedia</see></param>
public sealed record Instrument(
	string Id,
	string Ticker,
	string Company,
	string Country,
	string Currency,
	string Exchange,
	string MicCode,
	string CfiCode
);

public sealed record CompanyStats(
	decimal MarketCap,
	decimal Revenue
);

public sealed record StockData(
	Instrument Instrument,
	CompanyStats CompanyStats,
	CompanyReports CompanyReports,
	Frame<string, Bar> Prices,
	Frame<string, Bar> PricesUnadj
);
