namespace Feed.SEC;

public sealed record StringyRowSet(
	NumStringyRow[] Nums,
	PreStringyRow[] Pres,
	SubStringyRow[] Subs,
	TagStringyRow[] Tags
);