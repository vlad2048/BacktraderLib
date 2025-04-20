namespace Feed.Trading212._sys.Structs;

sealed record SearchItem(
	string FullName,
	string ShortName,
	string Exchange
);