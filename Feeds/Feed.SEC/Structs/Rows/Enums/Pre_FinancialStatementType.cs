namespace Feed.SEC;

public enum FinancialStatementType
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