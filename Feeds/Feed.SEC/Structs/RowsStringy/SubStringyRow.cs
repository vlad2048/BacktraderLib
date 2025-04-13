namespace Feed.SEC;

public sealed record SubStringyRow(
	Loc Loc,
	SubStringyRowKey Key,
	string Cik,
	string Name,
	string Sic,
	string Former,
	string Changed,
	string Afs,
	string Wksi,
	string Fye,
	string Form,
	string Period,
	string Fy,
	string Fp,
	string Filed,
	string Accepted,
	string Prevrpt,
	string Detail,
	string Instance
) : IStringyRow<SubStringyRow>
{
	public static string NameInArchive => "sub.txt";
	public static int ColumnCount => 36;
	public static SubStringyRow Parse(Loc loc, string[] xs) => new(
		loc,
		new SubStringyRowKey(xs[0]),
		xs[1],
		xs[2],
		xs[3],
		xs[20],
		xs[21],
		xs[22],
		xs[23],
		xs[24],
		xs[25],
		xs[26],
		xs[27],
		xs[28],
		xs[29],
		xs[30],
		xs[31],
		xs[32],
		xs[33]
	);
}