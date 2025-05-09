using System.Runtime.CompilerServices;
using Feed.Symbology;

namespace Feed.Final;

/// <param name="Figi">TwelveDataSymbol.FigiCode (unique)</param>
/// <param name="Isin">Trading212Symbol.Isin</param>
/// <param name="Mic">TwelveDataSymbol.MicCode</param>
/// <param name="Cfi">TwelveDataSymbol.CfiCode</param>
/// <param name="Exchange">TwelveDataSymbol.Exchange</param>
/// <param name="Exchange_Trading212">Trading212Symbol.Exchange</param>
/// <param name="ScheduleId_Trading212">Trading212Symbol.WorkingScheduleId</param>
/// 
/// <param name="Ticker">Trading212Symbol.Name = TwelveDataSymbol.Name</param>
/// <param name="Ccy">Trading212Symbol.Ccy</param>
/// <param name="Country">TwelveDataSymbol.Country</param>
/// 
/// <param name="Name_Trading212">Trading212Symbol.Name</param>
/// <param name="Name_TwelveData">TwelveDataSymbol.Name</param>
public record Symbol(
	string Figi,
	string Isin,
	string Mic,
	string Cfi,
	string Exchange,
	string Exchange_Trading212,
	int ScheduleId_Trading212,

	string Ticker,
	string Ccy,
	string Country,

	string Name_Trading212,
	string Name_TwelveData
)
{
	internal static Symbol Make(Trading212Symbol t2, TwelveDataSymbol td) => new(
		td.FigiCode.NotEmpty(),
		t2.Isin.NotEmpty(),
		td.MicCode.NotEmpty(),
		td.CfiCode.NotEmpty(),
		td.Exchange.NotEmpty(),
		t2.Exchange.NotEmpty(),
		t2.WorkingScheduleId,

		t2.ShortName.EnsureEqual(td.Symbol),
		t2.Ccy.NotEmpty(),
		td.Country.NotEmpty(),

		t2.Name.NotEmpty(),
		td.Name.NotEmpty()
	);
}


file static class SymbolUtils
{
	public static string EnsureEqual(this string a, string b, [CallerArgumentExpression(nameof(a))] string? aName = null, [CallerArgumentExpression(nameof(b))] string? bName = null)
	{
		if (a != b)
			throw new ArgumentException($"Expected {aName} to be equal to {bName} ({a} / {b})");
		return a;
	}

	public static string NotEmpty(this string a, [CallerArgumentExpression(nameof(a))] string? aName = null)
	{
		if (string.IsNullOrWhiteSpace(a))
			throw new ArgumentException($"Expected {aName} to be not empty ({a})");
		return a;
	}
}