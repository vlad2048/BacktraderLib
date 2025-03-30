namespace Feed.SEC;

public sealed record IncomeStatement(
	[property: StmtField("Revenue", true, 0)] decimal Revenue,
	[property: StmtField("Cost of revenue", false, 0)] decimal CostOfRevenue,
	[property: StmtField("Gross profit", true, 0)] decimal GrossProfit,
	[property: StmtField("Gross profit margin", false, 1)] decimal GrossProfitMargin,
	[property: StmtField("Operating expenses", false, 0)] decimal OperatingExpenses,
	[property: StmtField("Selling, general and administrative expenses", false, 1)] decimal SellingGeneralAndAdministrativeExpenses,
	[property: StmtField("Research and development expenses", false, 1)] decimal ResearchAndDevelopmentExpenses,
	[property: StmtField("Other expenses", false, 1)] decimal OtherExpenses,
	[property: StmtField("Operating income", true, 0)] decimal OperatingIncome,
	[property: StmtField("Operating profit margin", false, 1)] decimal OperatingProfitMargin,
	[property: StmtField("Other income expenses net", false, 0)] decimal OtherIncomeExpensesNet,
	[property: StmtField("Income before tax (EBT)", true, 0)] decimal IncomeBeforeTaxEBT,
	[property: StmtField("EBT margin", false, 1)] decimal EBTMargin,
	[property: StmtField("Income tax expense", false, 0)] decimal IncomeTaxExpense,
	[property: StmtField("Net income", true, 0)] decimal NetIncome,
	[property: StmtField("Net profit margin", false, 1)] decimal NetProfitMargin,
	[property: StmtField("Earnings per share", false, 0)] decimal EarningsPerShare,
	[property: StmtField("Earnings per share (diluted)", false, 0)] decimal EarningsPerShareDiluted,
	[property: StmtField("Weighted average shares outstanding", false, 0)] decimal WeightedAverageSharesOutstanding,
	[property: StmtField("Weighted average shares outstanding (diluted)", false, 0)] decimal WeightedAverageSharesOutstandingDiluted
) : IStmt;