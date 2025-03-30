namespace Feed.SEC._sys.RowsStringy;

sealed record TagStringyRowKey(
	string Tag,
	string Version
);

sealed record TagStringyRow(
	Loc Loc,
	TagStringyRowKey Key,
	string Custom,
	string Abstract,
	string Datatype,
	string IsInstant,
	string Crdr,
	string Tlabel,
	string Doc
) : IStringyRow<TagStringyRow>
{
	public static string NameInArchive => "tag.txt";
	public static int ColumnCount => 9;
	public static TagStringyRow Parse(Loc loc, string[] xs) => new(
		loc,
		new TagStringyRowKey(
			xs[0],
			xs[1]
		),
		xs[2],
		xs[3],
		xs[4],
		xs[5],
		xs[6],
		xs[7],
		xs[8]
	);
}