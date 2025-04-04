/*
using System.Text.Json.Serialization;

namespace Feed.Trading212._sys.StateLogic.Structs;

sealed record SymbolState(
	DateTime ScrapeTime,
	string? ErrorMessage
)
{
	[JsonIgnore]
	public bool IsComplete => ErrorMessage == null;
}
*/