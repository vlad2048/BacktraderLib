using Feed.SEC._sys.Rows.Utils;

namespace Feed.SEC;

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
public sealed record NumRow(
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