namespace Feed.SEC;

public sealed record NumStringyRowKey(
	SubStringyRowKey SubKey,
	TagStringyRowKey TagKey,
	string DDate,
	string Qtrs,
	string Uom,
	string Segments,
	string Coreg
);