namespace Feed.TwelveData._sys.Structs;

sealed record TwelveDataError(
	int Code,
	string Message,
	string Status
)
{
	public override string ToString() => $"[{Code}] {Message} ({Status})";
}