using Feed.SEC._sys.Rows.Base;
using Feed.SEC._sys.Rows.Utils;
using Feed.SEC._sys.RowsStringy;

namespace Feed.SEC._sys.Rows;


sealed record NumRowKey(
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


/// <param name="Key.AdshKey.Adsh">0001558370-24-011687</param>
/// <param name="Key.TagVersionKey.Tag">AdditionalPaidInCapital</param>
/// <param name="Key.TagVersionKey.Version">us-gaap/2024</param>
/// <param name="Key.DDate">20240630 (always last day of the month)</param>
/// <param name="Key.Qtrs">2 or 0</param>
/// <param name="Key.Uom">USD</param>
/// <param name="Key.Segments">ClassOfStock=SeriesAPreferredStock;EquityComponents=PreferredStock;</param>
/// <param name="Key.Coreg">MidamericanEnergyCompanyAndSubsidiaries</param>
/// <param name="Value">460000000</param>
/// <param name="Footnote">These investments have a maturity date prior to the end of the current period. Sonar Entertainment is expected to be paid down in a series of payments subsequent to the stated maturity date. Additional proceeds are expected from Crowne Automotive and Solarplicity Group after the resolution of bankruptcy proceedings, or other corporate actions, at each respective issuer. | Non-accrual status (See Note 2 to the consolidated financial statements).</param>
sealed record NumRow(
	NumRowKey Key,
	decimal? Value,
	string? Footnote
) : IRow<NumRow>
{
	public static string NameInArchive => "num.txt";
	public static int ColumnCount => 10;
	public static string HeaderLine => "adsh	tag	version	ddate	qtrs	uom	segments	coreg	value	footnote";

	public static bool IsParsable(NumStringyRow e) =>
		e.Key.SubKey.Adsh.Is_String() &&
		e.Key.TagKey.Tag.Is_String() &&
		e.Key.TagKey.Version.Is_String() &&
		e.Key.DDate.Is_DateOnlyCompact() &&
		e.Key.Qtrs.Is_Int() &&
		e.Key.Uom.Is_String() &&
		e.Key.Segments.Is_NullableString() &&
		e.Key.Coreg.Is_NullableString() &&
		e.Value.Is_NullableDecimal() &&
		e.Footnote.Is_NullableString();

	public NumPreXRef NumPreXRef => new(Key.SubKey.Adsh, Key.TagKey.Tag, Key.TagKey.Version);

	// @formatter:off
	public static NumRow Parse(string[] xs) => new(
		Key:		new NumRowKey(
						SubKey:		new SubRowKey(xs[0].As_String()),
						TagKey:		new TagRowKey(xs[1].As_String(), xs[2].As_String()),
						DDate:		xs[3].As_DateOnlyCompact(),
						Qtrs:		xs[4].As_Int(),
						Uom:		xs[5],
						Segments:	xs[6].As_NullableString(),
						Coreg:		xs[7].As_NullableString()
					),
		Value:		xs[8].As_NullableDecimal(),
		Footnote:	xs[9].As_NullableString()
	);
	// @formatter:on

	public string?[] Fields => [
		Key.SubKey.Adsh.Fmt_String(),
		Key.TagKey.Tag.Fmt_String(),
		Key.TagKey.Version.Fmt_String(),
		Key.DDate.Fmt_DateOnlyCompact(),
		Key.Qtrs.Fmt_Int(),
		Key.Uom.Fmt_String(),
		Key.Segments.Fmt_NullableString(),
		Key.Coreg.Fmt_NullableString(),
		Value.Fmt_NullableDecimal(),
		Footnote.Fmt_NullableString(),
	];


	public object ToDump() => new
	{
		Adsh = Key.SubKey,
		Tag = Key.TagKey,
		Key.DDate,
		Key.Qtrs,
		Key.Uom,
		Key.Segments,
		Key.Coreg,
		Value = Value switch
		{
			null => "_",
			_ => $"{Value:n0}",
		},
		Footnote,
	};
}






/*
sealed record NumRowKey(
	string Adsh,
	string Tag,
	string Version
);


/// <param name="Adsh">0001558370-24-011687</param>
/// <param name="Tag">AdditionalPaidInCapital</param>
/// <param name="Version">us-gaap/2024</param>
/// <param name="Date">20240630 (always last day of the month)</param>
/// <param name="Quarters">2 or 0</param>
/// <param name="Uom">USD</param>
/// <param name="Segments">ClassOfStock=SeriesAPreferredStock;EquityComponents=PreferredStock;</param>
/// <param name="Coreg">MidamericanEnergyCompanyAndSubsidiaries</param>
/// <param name="Value">460000000</param>
/// <param name="Footnote">These investments have a maturity date prior to the end of the current period. Sonar Entertainment is expected to be paid down in a series of payments subsequent to the stated maturity date. Additional proceeds are expected from Crowne Automotive and Solarplicity Group after the resolution of bankruptcy proceedings, or other corporate actions, at each respective issuer. | Non-accrual status (See Note 2 to the consolidated financial statements).</param>
sealed record NumRow(
	string Adsh,
	string Tag,
	string Version,

	//string DDate,
	//string Qtrs,

	//Period Period,

	DateOnly Date,
	int Quarters,

	string Uom,
	string? Segments,
	string? Coreg,
	decimal? Value,
	string? Footnote
) : IRow<NumRow, NumRowKey>
{
	public static string ArchiveFileName => "num.txt";

	public static bool IsValid(string[] x) =>
		x[3] != ""     // ddate
	;

	public NumRowKey GetKey() => new(Adsh, Tag, Version);
	public static NumRowKey ParseKey(string[] x) => new(x[0], x[1], x[2]);

	public static NumRow Parse(string[] x)
	{
		var adsh = x[0];
		var tag = x[1];
		var version = x[2];
		var ddate = x[3];
		var qtrs = x[4];
		var uom = x[5];
		var segments = x[6];
		var coreg = x[7];
		var value = x[8];
		var footnote = x[9];


		var ddate_Parse = ddate.ParseCompactDateOnly();
		var qtrs_Parse = int.Parse(qtrs);

		var segments_Parse = segments.Parse_StringOpt();
		var coreg_Parse = coreg.Parse_StringOpt();
		var value_Parse = value.TryParse_DecimalOpt();
		var footnote_Parse = footnote.Parse_StringOpt();


		return new NumRow(
			adsh,
			tag,
			version,

			ddate_Parse,
			qtrs_Parse,

			uom,
			segments_Parse,
			coreg_Parse,
			value_Parse,
			footnote_Parse
		);
	}
}
*/