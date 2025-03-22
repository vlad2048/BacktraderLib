using BaseUtils;

namespace Feed.Yahoo._sys.Errors;

sealed record YahooExceptionNfo(
	YahooExceptionType Type,
	string[] Messages
)
{
	public override string ToString() => $"[{Type}] - {Messages.JoinText("==(inner)==>")}";
}