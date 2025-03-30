namespace Feed.SEC;


public sealed record BalanceSheet(
	[property: StmtField("Total assets", true, 0)] decimal TotalAssets,

	[property: StmtField("Total current assets", true, 0)] decimal TotalCurrentAssets,
	[property: StmtField("Cash and short term investments", false, 1)] decimal CashAndShortTermInvestments,
	[property: StmtField("Cash and cash equivalents", false, 2)] decimal CashAndCashEquivalents,
	[property: StmtField("Short term investments", false, 2)] decimal ShortTermInvestments,
	[property: StmtField("Receivables", false, 1)] decimal Receivables,
	[property: StmtField("Inventory", false, 1)] decimal Inventory,
	[property: StmtField("Other current assets", false, 1)] decimal OtherCurrentAssets,

	[property: StmtField("Total non-current assets", true, 0)] decimal TotalNonCurrentAssets,
	[property: StmtField("Property, plant & equipment net", false, 1)] decimal PropertyPlantAndEquipmentNet,
	[property: StmtField("Goodwill and intangible assets", false, 1)] decimal GoodwillAndIntangibleAssets,
	[property: StmtField("Goodwill", false, 2)] decimal Goodwill,
	[property: StmtField("Intangible assets", false, 2)] decimal IntangibleAssets,
	[property: StmtField("Long term investments", false, 1)] decimal LongTermInvestments,
	[property: StmtField("Other non-current assets", false, 1)] decimal OtherNonCurrentAssets,

	[property: StmtField("Other assets", true, 0)] decimal OtherAssets,

	[property: StmtField("Total liabilities and equity", true, 0)] decimal TotalLiabilitiesAndEquity,

	[property: StmtField("Total liabilities", true, 0)] decimal TotalLiabilities,

	[property: StmtField("Current liabilities", true, 0)] decimal CurrentLiabilities,
	[property: StmtField("Payables", false, 1)] decimal Payables,
	[property: StmtField("Short term debt", false, 1)] decimal ShortTermDebt,
	[property: StmtField("Deferred revenue", false, 1)] decimal DeferredRevenue,
	[property: StmtField("Other current liabilities", false, 1)] decimal OtherCurrentLiabilities,

	[property: StmtField("Non-current liabilities", true, 0)] decimal NonCurrentLiabilities,
	[property: StmtField("Long term debt", false, 1)] decimal LongTermDebt,
	[property: StmtField("Other non-current liabilities", false, 1)] decimal OtherNonCurrentLiabilities,
	[property: StmtField("Non-current deferred revenue", false, 1)] decimal NonCurrentDeferredRevenue,
	[property: StmtField("Non-current deferred tax liabilities", false, 1)] decimal NonCurrentDeferredTaxLiabilities,

	[property: StmtField("Total equity", true, 0)] decimal TotalEquity,
	[property: StmtField("Common stock", false, 1)] decimal CommonStock,
	[property: StmtField("Retained earnings", false, 1)] decimal RetainedEarnings,
	[property: StmtField("Other stockholders equity", false, 1)] decimal OtherStockholdersEquity,
	[property: StmtField("Accumulated other comprehensive income loss", false, 1)] decimal AccumulatedOtherComprehensiveIncomeLoss,
	[property: StmtField("Minority interest", false, 1)] decimal MinorityInterest
) : IStmt;
