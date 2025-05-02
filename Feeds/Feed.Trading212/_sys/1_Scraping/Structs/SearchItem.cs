namespace Feed.Trading212._sys._1_Scraping.Structs;

sealed record SearchItem(
	string FullName,
	string ShortName,
	string Exchange
);