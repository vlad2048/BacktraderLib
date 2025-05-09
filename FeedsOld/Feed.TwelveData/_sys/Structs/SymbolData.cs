namespace Feed.TwelveData._sys.Structs;

// ReSharper disable once UnusedMember.Global
sealed record SymbolData(
	DateTime LastUpdate,
	TwelveDataBar[] Bars
);