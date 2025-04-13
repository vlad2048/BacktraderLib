namespace Feed.SEC;

public sealed record NumStringyRow(
	Loc Loc,
	NumStringyRowKey Key,
	string Value,
	string Footnote
) : IStringyRow<NumStringyRow>
{
	public static string NameInArchive => "num.txt";
	public static int ColumnCount => 10;
	public static NumStringyRow Parse(Loc loc, string[] xs) => new(
		loc,
		new NumStringyRowKey(
			new SubStringyRowKey(xs[0]),
			new TagStringyRowKey(xs[1], xs[2]),
			xs[3],
			xs[4],
			xs[5],
			xs[6],
			xs[7]
		),
		xs[8],
		xs[9]
	);
}


