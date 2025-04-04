namespace Feed.Trading212;

public sealed record SearchItem(
	string FullName,
	string ShortName,
	string Exchange
);