using Feed.SEC._sys.Rows.Utils;

namespace Feed.SEC;

public sealed record PreRow(
	PreRowKey Key,
	TagRowKey TagKey,
	FinancialStatementType Stmt,
	bool Inpth,
	RFile Rfile,
	string? Plabel,
	bool Negating
) : IRow<PreRow>
{
	public static string NameInArchive => "pre.txt";
	public static int ColumnCount => 10;
	public static string HeaderLine => "adsh	report	line	stmt	inpth	rfile	tag	version	plabel	negating";

	public static bool IsParsable(PreStringyRow e) =>
		e.Key.SubKey.Adsh.Is_String() &&
		e.Key.Report.Is_Int() &&
		e.Key.Line.Is_Int() &&
		e.TagKey.Tag.Is_String() &&
		e.TagKey.Version.Is_String() &&
		e.Stmt.Is_FinancialStatementType() &&
		e.Inpth.Is_Bool() &&
		e.Rfile.Is_RFile() &&
		e.Plabel.Is_NullableString() &&
		e.Negating.Is_Bool();

	public NumPreXRef NumPreXRef => new(Key.SubKey.Adsh, TagKey.Tag, TagKey.Version);

	// @formatter:off
	public static PreRow Parse(string[] xs) => new(
		Key:	new PreRowKey(
					SubKey:	new SubRowKey(xs[0].As_String()),
					Report:	xs[1].As_Int(),
					Line:	xs[2].As_Int()
				),
		TagKey:	new TagRowKey(
					xs[6].As_String(),
					xs[7].As_String()
				),
		xs[3].As_FinancialStatementType(),
		xs[4].As_Bool(),
		xs[5].As_RFile(),
		xs[8].As_NullableString(),
		xs[9].As_Bool()
	);
	// @formatter:on

	public string?[] Fields => [
		Key.SubKey.Adsh.Fmt_String(),
		Key.Report.Fmt_Int(),
		Key.Line.Fmt_Int(),
		Stmt.Fmt_FinancialStatementType(),
		Inpth.Fmt_Bool(),
		Rfile.Fmt_RFile(),
		TagKey.Tag.Fmt_String(),
		TagKey.Version.Fmt_String(),
		Plabel.Fmt_NullableString(),
		Negating.Fmt_Bool(),
	];


	public object ToDump() => new
	{
		Key.SubKey.Adsh,
		Tag = TagKey,
		Stmt,
		Inpth,
		Rfile,
		ReportLine = $"{Key.Report}:{Key.Line}",
		Plabel,
		Negating,
	};
}