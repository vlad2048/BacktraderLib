namespace Feed.Trading212;

public sealed record SymbolDef(
	SearchItem Item,
	string SECCompany
)
{
	public override string ToString() => SECCompany;
}