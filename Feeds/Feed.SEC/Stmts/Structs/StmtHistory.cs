using BaseUtils;

namespace Feed.SEC;

public sealed record StmtHistory(
	Dictionary<Quarter, IncomeStatement> IncomeStatements,
	Dictionary<Quarter, BalanceSheet> BalanceSheets,
	Dictionary<Quarter, CashFlow> CashFlows
)
{
	public static readonly StmtHistory Empty = new([], [], []);
}