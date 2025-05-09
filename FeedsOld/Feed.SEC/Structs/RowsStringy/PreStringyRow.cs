namespace Feed.SEC;

public sealed record PreStringyRow(
	Loc Loc,
	PreStringyRowKey Key,
	TagStringyRowKey TagKey,
	string Stmt,
	string Inpth,
	string Rfile,
	string Plabel,
	string Negating
) : IStringyRow<PreStringyRow>
{
	public static string NameInArchive => "pre.txt";
	public static int ColumnCount => 10;
	public static PreStringyRow Parse(Loc loc, string[] xs) => new(
		loc,
		new PreStringyRowKey(
			new SubStringyRowKey(xs[0]),
			xs[1],
			xs[2]
		),
		new TagStringyRowKey(
			xs[6],
			xs[7]
		),
		xs[3],
		xs[4],
		xs[5],
		xs[8],
		xs[9]
	);
}