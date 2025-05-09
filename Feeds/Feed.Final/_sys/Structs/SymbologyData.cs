using Feed.Symbology;

namespace Feed.Final._sys.Structs;

public sealed record SymbologyData(
	Trading212SymbolData Trading212,
	TwelveDataSymbol[] TwelveData,
	Mic[] Mics
);