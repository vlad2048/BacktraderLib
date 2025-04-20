namespace BaseUtils;

public sealed record CompanyDef(
	string Name,
	string Exchange,
	string MainTicker,
	decimal MarketCap,
	decimal Revenue
);