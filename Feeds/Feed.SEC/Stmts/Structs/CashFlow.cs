namespace Feed.SEC;

public sealed record CashFlow(
	[property: StmtField("Operating cash flow", true, 0)] decimal OperatingCashFlow,
	[property: StmtField("Net income", false, 1)] decimal NetIncome,
	[property: StmtField("Depreciation & amortization", false, 1)] decimal DepreciationAmortization,
	[property: StmtField("Deferred income taxes", false, 1)] decimal DeferredIncomeTaxes,
	[property: StmtField("Stock-based compensation", false, 1)] decimal StockBasedCompensation,
	[property: StmtField("Change in working capital", false, 1)] decimal ChangeInWorkingCapital,
	[property: StmtField("Other non-cash items", false, 1)] decimal OtherNonCashItems,

	[property: StmtField("Investing cash flow", true, 0)] decimal InvestingCashFlow,
	[property: StmtField("Investments in PPE", false, 1)] decimal InvestmentsInPPE,
	[property: StmtField("Acquisitions", false, 1)] decimal Acquisitions,
	[property: StmtField("Investment purchases", false, 1)] decimal InvestmentPurchases,
	[property: StmtField("Sales/maturities of investments", false, 1)] decimal SalesMaturitiesOfInvestments,
	[property: StmtField("Other investing activites", false, 1)] decimal OtherInvestingActivites,

	[property: StmtField("Financing cash flow", true, 0)] decimal FinancingCashFlow,
	[property: StmtField("Debt repayment", false, 1)] decimal DebtRepayment,
	[property: StmtField("Dividends payments", false, 1)] decimal DividendsPayments,
	[property: StmtField("Common stock repurchased", false, 1)] decimal CommonStockRepurchased,
	[property: StmtField("Common stock issuance", false, 1)] decimal CommonStockIssuance,
	[property: StmtField("Other financing activities", false, 1)] decimal OtherFinancingActivities,

	[property: StmtField("Effect of forex changes on cash", true, 0)] decimal EffectOfForexChangesOnCash,

	[property: StmtField("Change in cash", true, 0)] decimal ChangeInCash,
	[property: StmtField("Cash at beginning of period", false, 1)] decimal CashAtBeginningOfPeriod,
	[property: StmtField("Cash at end of period", false, 1)] decimal CashAtEndOfPeriod,
	[property: StmtField("Capital expenditure", false, 0)] decimal CapitalExpenditure,
	[property: StmtField("Free cash flow", false, 0)] decimal FreeCashFlow
) : IStmt;