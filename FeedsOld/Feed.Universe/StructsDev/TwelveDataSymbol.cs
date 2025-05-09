using System.Text.Json.Serialization;

namespace Feed.Universe;

public sealed record TwelveDataSymbolKey(
	string Symbol,
	string Exchange,
	string Country
)
{
	public override string ToString() => $"{Symbol} ({Exchange} / {Country})";
	public object ToDump() => ToString();
}

public sealed record TwelveDataSymbol(
	string Symbol,
	string Exchange,
	string Country,
	string Name,
	string Currency,
	string MicCode,
	string Type,
	string FigiCode,
	string CfiCode
)
{
	[JsonIgnore]
	public TwelveDataSymbolKey Key => new(Symbol, Exchange, Country);

	public object ToDump() => new
	{
		Symbol,
		Exchange,
		Country,
		Name,
		Currency,
		Type,
		MicCode,
		FigiCode,
		CfiCode,
	};
}