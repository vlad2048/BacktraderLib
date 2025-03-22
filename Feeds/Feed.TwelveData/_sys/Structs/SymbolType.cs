using System.Text.Json.Serialization;

namespace Feed.TwelveData._sys.Structs;

enum SymbolType
{
	[JsonStringEnumMemberName("American Depositary Receipt")] AmericanDepositaryReceipt,
	[JsonStringEnumMemberName("Bond")] Bond,
	[JsonStringEnumMemberName("Bond Fund")] BondFund,
	[JsonStringEnumMemberName("Closed-end Fund")] ClosedEndFund,
	[JsonStringEnumMemberName("Common Stock")] CommonStock,
	[JsonStringEnumMemberName("Depositary Receipt")] DepositaryReceipt,
	[JsonStringEnumMemberName("Digital Currency")] DigitalCurrency,
	[JsonStringEnumMemberName("ETF")] ETF,
	[JsonStringEnumMemberName("Exchange-Traded Note")] ExchangeTradedNote,
	[JsonStringEnumMemberName("Global Depositary Receipt")] GlobalDepositaryReceipt,
	[JsonStringEnumMemberName("Index")] Index,
	[JsonStringEnumMemberName("Limited Partnership")] LimitedPartnership,
	[JsonStringEnumMemberName("Mutual Fund")] MutualFund,
	[JsonStringEnumMemberName("Physical Currency")] PhysicalCurrency,
	[JsonStringEnumMemberName("Preferred Stock")] PreferredStock,
	[JsonStringEnumMemberName("REIT")] REIT,
	[JsonStringEnumMemberName("Right")] Right,
	[JsonStringEnumMemberName("Structured Product")] StructuredProduct,
	[JsonStringEnumMemberName("Trust")] Trust,
	[JsonStringEnumMemberName("Unit")] Unit,
	[JsonStringEnumMemberName("Warrant")] Warrant,

	[JsonStringEnumMemberName("Agricultural Product")] AgriculturalProduct,
	[JsonStringEnumMemberName("Energy Resource")] EnergyResource,
	[JsonStringEnumMemberName("Industrial Metal")] IndustrialMetal,
	[JsonStringEnumMemberName("Livestock")] Livestock,
	[JsonStringEnumMemberName("Precious Metal")] PreciousMetal,
}
