using System.Text.Json.Serialization;

namespace Feed.TwelveData._sys.Structs;

[JsonDerivedType(typeof(StockDef), 0)]
[JsonDerivedType(typeof(ForexPairDef), 1)]
[JsonDerivedType(typeof(CryptocurrencyDef), 2)]
[JsonDerivedType(typeof(FundDef), 3)]
[JsonDerivedType(typeof(BondDef), 4)]
[JsonDerivedType(typeof(ETFDef), 5)]
[JsonDerivedType(typeof(IndexDef), 6)]
[JsonDerivedType(typeof(CommodityDef), 7)]
interface ISymbolDef
{
	// ReSharper disable once UnusedMember.Global
	string Symbol { get; }
}

sealed record StockDef(
	string Symbol,
	string Name,
	string Country,
	string Currency,
	string Exchange,
	SymbolType Type,
	string FigiCode,
	string MicCode,
	Plan Access
) : ISymbolDef;

sealed record ForexPairDef(
	string Symbol,
	string CurrencyGroup,
	string CurrencyBase,
	string CurrencyQuote
) : ISymbolDef;

sealed record CryptocurrencyDef(
	string Symbol,
	string[] AvailableExchanges,
	string CurrencyBase,
	string CurrencyQuote
) : ISymbolDef;

sealed record FundDef(
	string Symbol,
	string Name,
	string Country,
	string Currency,
	string Exchange,
	SymbolType Type,
	string FigiCode,
	string MicCode,
	Plan Access
) : ISymbolDef;

sealed record BondDef(
	string Symbol,
	string Name,
	string Country,
	string Currency,
	string Exchange,
	string MicCode,
	SymbolType Type,
	Plan Access
) : ISymbolDef;

sealed record ETFDef(
	string Symbol,
	string Name,
	string Country,
	string Currency,
	string Exchange,
	string FigiCode,
	string MicCode,
	Plan Access
) : ISymbolDef;

sealed record IndexDef(
	string Symbol,
	string Name,
	string Country,
	string Currency,
	string Exchange,
	string MicCode
) : ISymbolDef;

sealed record CommodityDef(
	string Symbol,
	string Name,
	string Category,
	string Description
) : ISymbolDef;

