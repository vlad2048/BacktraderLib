using BaseUtils;

namespace Feed.Trading212;


public record SymbolData(
	string Company,
	DateTime ScrapeTime,
	string? ErrorMessage,
	Dictionary<ReportType, SortedDictionary<Quarter, RefField[]>> Reports
);


public sealed record RefField(
	string Title,
	RefValue Value,
	bool Important,
	RefField[]? Rows
);


public enum RefValueType
{
	Currency,
	Percent,
	Number,
	Text,
}

public sealed record RefValue(
	decimal? Value,
	bool Compact,
	string? Currency,
	RefValueType Type
)
{
	public override string ToString() => this.Fmt();
	public object ToDump() => ToString();
}



file static class RefValueFormatter
{
	public static string Fmt(this RefValue e) =>
		e.Type switch
		{
			RefValueType.Currency => FmtCurrency(e.Value.Force(), e.Currency.Force()),
			RefValueType.Percent => $"{e.Value.Force() * 100:F2}%",
			RefValueType.Number => e.Value.Force().Fmt(),
			RefValueType.Text => "_",
			_ => throw new ArgumentException($"Unknown RefValueType: {e.Type}"),
		};

	static decimal Force(this decimal? v) => v ?? throw new ArgumentException("Missing Value");
	static string Force(this string? v) => v ?? throw new ArgumentException("Missing Currency");

	static string FmtCurrency(decimal v, string ccy) =>
		ccy switch
		{
			"USD" => $"${v.Fmt()}",
			_ => $"{v.Fmt()}{ccy}",
		};


	static string Fmt(this decimal v) =>
		Math.Abs(v) switch
		{
			>= 1_000_000_000 => $"{v / 1_000_000_000:F2}B",
			>= 1_000_000 => $"{v / 1_000_000:F2}M",
			>= 1_000 => $"{v / 1_000:F2}K",
			_ => $"{v:F2}",
		};
}