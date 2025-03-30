using Feed.SEC._sys.Rows.Base;
using Feed.SEC._sys.Rows.Utils;
using Feed.SEC._sys.RowsStringy;
using Feed.SEC._sys.Utils;

namespace Feed.SEC._sys.Rows;




sealed record TagRowKey(
	string Tag,
	string Version
)
{
	public override string ToString() => $"{Tag}@{FmtVersion(Version)}".Ellipse(Consts.DumpCaps.Tag);
	public object ToDump() => $"{this}";


	const string OfficialVersionPrefix = "us-gaap/";

	static string FmtVersion(string version) =>
		version.StartsWith(OfficialVersionPrefix) switch
		{
			true => $"({version[OfficialVersionPrefix.Length..]})",
			false => version,
		};
}


/// <param name="Key.Tag">AccountsPayableCurrent</param>
/// <param name="Key.Version">us-gaap/2023</param>
/// <param name="Custom">0/1</param>
/// <param name="Datatype">monetary</param>
/// <param name="IsInstant">D/I</param>
/// <param name="Crdr">C/D</param>
/// <param name="Tlabel">Accounts payable</param>
/// <param name="Doc">Carrying value as of the balance sheet date of obligations incurred through that date and payable for professional fees, such as for legal and accounting services received. Used to reflect the current portion of the liabilities (due within one year or within the normal operating cycle if longer).</param>
sealed record TagRow(
	TagRowKey Key,
	bool Custom,
	bool Abstract,
	Datatype Datatype,
	bool IsInstant,
	Cred? Crdr,
	string? Tlabel,
	string? Doc
) : IRow<TagRow>
{
	public static string NameInArchive => "tag.txt";
	public static int ColumnCount => 9;
	public static string HeaderLine => "tag	version	custom	abstract	datatype	iord	crdr	tlabel	doc";

	public static bool IsParsable(TagStringyRow e) =>
		e.Key.Tag.Is_String() &&
		e.Key.Version.Is_String() &&
		e.Custom.Is_Bool() &&
		e.Abstract.Is_Bool() &&
		e.Datatype.Is_Datatype() &&
		e.IsInstant.Is_BoolInstant() &&
		e.Crdr.Is_Cred() &&
		e.Tlabel.Is_NullableString() &&
		e.Doc.Is_NullableString();

	// @formatter:off
	public static TagRow Parse(string[] xs) => new(
		Key: new TagRowKey(
			Tag:		xs[0].As_String(),
			Version:	xs[1].As_String()
		),
		Custom:		xs[2].As_Bool(),
		Abstract:	xs[3].As_Bool(),
		Datatype:	xs[4].As_Datatype(),
		IsInstant:	xs[5].As_BoolInstant(),
		Crdr:		xs[6].As_Cred(),
		Tlabel:		xs[7].As_NullableString(),
		Doc:		xs[8].As_NullableString()
	);
	// @formatter:on

	public string?[] Fields => [
		Key.Tag.Fmt_String(),
		Key.Version.Fmt_String(),
		Custom.Fmt_Bool(),
		Abstract.Fmt_Bool(),
		Datatype.Fmt_Datatype(),
		IsInstant.Fmt_BoolInstant(),
		Crdr.Fmt_Cred(),
		Tlabel.Fmt_NullableString(),
		Doc.Fmt_NullableString(),
	];


	public object ToDump() => new
	{
		Key,
		Custom,
		Abstract,
		Datatype,
		IsInstant,
		Crdr,
		Tlabel = Tlabel.Ellipse(Consts.DumpCaps.Tag_Tlabel),
		Doc = Doc.Ellipse(Consts.DumpCaps.Tag_Doc),
	};
}