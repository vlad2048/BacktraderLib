namespace Feed.SEC;

public sealed record NumRowKey(
	SubRowKey SubKey,
	TagRowKey TagKey,
	DateOnly DDate,
	int Qtrs,
	string Uom,
	string? Segments,
	string? Coreg
)
{
	public override string ToString() => $"{SubKey} - {TagKey.Tag}";
	public object ToDump() => $"{this}";
}