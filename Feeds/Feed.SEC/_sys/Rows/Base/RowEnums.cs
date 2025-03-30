namespace Feed.SEC._sys.Rows.Base;


// *************
// *************
// **** NUM ****
// *************
// *************




// *************
// *************
// **** PRE ****
// *************
// *************
enum FinancialStatementType
{
	BalanceSheet,
	IncomeStatement,
	CashFlow,
	Equity,
	ComprehensiveIncome,
	ScheduleOfInvestments,
	UnclassifiableStatement,

	// Undocumented and rare
	CP,
	None,
}

static class FinancialStatementTypeUtils
{
	public static bool Is_FinancialStatementType(this string s) => s is "BS" or "IS" or "CF" or "EQ" or "CI" or "SI" or "UN" or "CP" or "";

	public static FinancialStatementType As_FinancialStatementType(this string s) => s switch
	{
		"BS" => FinancialStatementType.BalanceSheet,
		"IS" => FinancialStatementType.IncomeStatement,
		"CF" => FinancialStatementType.CashFlow,
		"EQ" => FinancialStatementType.Equity,
		"CI" => FinancialStatementType.ComprehensiveIncome,
		"SI" => FinancialStatementType.ScheduleOfInvestments,
		"UN" => FinancialStatementType.UnclassifiableStatement,
		"CP" => FinancialStatementType.CP,
		"" => FinancialStatementType.None,
		_ => throw new ArgumentException($"Unrecognized [Pre].Stmt: '{s}'"),
	};

	public static string Fmt_FinancialStatementType(this FinancialStatementType s) => s switch
	{
		FinancialStatementType.BalanceSheet => "BS",
		FinancialStatementType.IncomeStatement => "IS",
		FinancialStatementType.CashFlow => "CF",
		FinancialStatementType.Equity => "EQ",
		FinancialStatementType.ComprehensiveIncome => "CI",
		FinancialStatementType.ScheduleOfInvestments => "SI",
		FinancialStatementType.UnclassifiableStatement => "UN",
		FinancialStatementType.CP => "CP",
		FinancialStatementType.None => "",
		_ => throw new ArgumentException("Unknown FinancialStatementType"),
	};
}

enum RFile
{
	Htm,
	Xml,
}

static class RFileUtils
{
	public static bool Is_RFile(this string s) => s is "H" or "X";
	public static RFile As_RFile(this string s) => s switch
	{
		"H" => RFile.Htm,
		"X" => RFile.Xml,
		_ => throw new ArgumentException($"Unrecognized [Pre].RFile: '{s}'"),
	};
	public static string Fmt_RFile(this RFile s) => s switch
	{
		RFile.Htm => "H",
		RFile.Xml => "X",
		_ => throw new ArgumentException($"Unrecognized [Pre].RFile: '{s}'"),
	};
}




// *************
// *************
// **** SUB ****
// *************
// *************
enum FilerStatus
{
	NotAssigned,
	LargeAccelerated,
	Accelerated,
	SmallerReportingAccelerated,
	NonAccelerated,
	SmallerReportingFiler,
}

static class FilerStatusUtils
{
	public static bool Is_FilerStatus(this string s) => s is "" or "1-LAF" or "2-ACC" or "3-SRA" or "4-NON" or "5-SML";
	public static FilerStatus As_FilerStatus(this string s) => s switch
	{
		"" => FilerStatus.NotAssigned,
		"1-LAF" => FilerStatus.LargeAccelerated,
		"2-ACC" => FilerStatus.Accelerated,
		"3-SRA" => FilerStatus.SmallerReportingAccelerated,
		"4-NON" => FilerStatus.NonAccelerated,
		"5-SML" => FilerStatus.SmallerReportingFiler,
		_ => throw new ArgumentException($"Not a recognized FilerStatus: '{s}'"),
	};
	public static string Fmt_FilerStatus(this FilerStatus s) => s switch
	{
		FilerStatus.NotAssigned => "",
		FilerStatus.LargeAccelerated => "1-LAF",
		FilerStatus.Accelerated => "2-ACC",
		FilerStatus.SmallerReportingAccelerated => "3-SRA",
		FilerStatus.NonAccelerated => "4-NON",
		FilerStatus.SmallerReportingFiler => "5-SML",
		_ => throw new ArgumentException($"Not a recognized FilerStatus: '{s}'"),
	};
}





// *************
// *************
// **** TAG ****
// *************
// *************
enum Cred
{
	Credit,
	Debit,
}

static class CredUtils
{
	public static bool Is_Cred(this string s) => s is "" or "C" or "D";
	public static Cred? As_Cred(this string s) => s switch
	{
		"" => null,
		"C" => Cred.Credit,
		"D" => Cred.Debit,
		_ => throw new ArgumentException($"Unknown Cred: '{s}'"),
	};
	public static string Fmt_Cred(this Cred? s) => s switch
	{
		null => "",
		Cred.Credit => "C",
		Cred.Debit => "D",
		_ => throw new ArgumentException($"Unknown Cred: '{s}'"),
	};
}



enum Datatype
{
	Monetary,
	PerShare,
	Shares,
	Percent,
	PerUnit,
	Decimal,
	Pure,
	Integer,
	PositiveInteger,
	Memory,
	Area,
	Volume,
	Mass,
	NonNegativeInteger,
	Power,
	MonetaryPerVolume,
	Energy,
}

static class DatatypeUtils
{
	public static bool Is_Datatype(this string s) => s is "monetary" or "perShare" or "shares" or "percent" or "perUnit" or "decimal" or "pure" or "integer" or "positiveInteger" or "memory" or "area" or "volume" or "mass" or "nonNegativeInteger" or "power" or "monetaryPerVolume" or "energy";
	public static Datatype As_Datatype(this string s) => s switch
	{
		"monetary" => Datatype.Monetary,
		"perShare" => Datatype.PerShare,
		"shares" => Datatype.Shares,
		"percent" => Datatype.Percent,
		"perUnit" => Datatype.PerUnit,
		"decimal" => Datatype.Decimal,
		"pure" => Datatype.Pure,
		"integer" => Datatype.Integer,
		"positiveInteger" => Datatype.PositiveInteger,
		"memory" => Datatype.Memory,
		"area" => Datatype.Area,
		"volume" => Datatype.Volume,
		"mass" => Datatype.Mass,
		"nonNegativeInteger" => Datatype.NonNegativeInteger,
		"power" => Datatype.Power,
		"monetaryPerVolume" => Datatype.MonetaryPerVolume,
		"energy" => Datatype.Energy,
		_ => throw new ArgumentException($"Unknown Datatype: '{s}'"),
	};
	public static string Fmt_Datatype(this Datatype e) => e switch
	{
		Datatype.Monetary => "monetary",
		Datatype.PerShare => "perShare",
		Datatype.Shares => "shares",
		Datatype.Percent => "percent",
		Datatype.PerUnit => "perUnit",
		Datatype.Decimal => "decimal",
		Datatype.Pure => "pure",
		Datatype.Integer => "integer",
		Datatype.PositiveInteger => "positiveInteger",
		Datatype.Memory => "memory",
		Datatype.Area => "area",
		Datatype.Volume => "volume",
		Datatype.Mass => "mass",
		Datatype.NonNegativeInteger => "nonNegativeInteger",
		Datatype.Power => "power",
		Datatype.MonetaryPerVolume => "monetaryPerVolume",
		Datatype.Energy => "energy",
		_ => throw new ArgumentException($"Unknown Datatype: '{e}'"),
	};
}