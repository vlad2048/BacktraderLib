using BaseUtils;
using Feed.SEC._sys.Rows.Base;
using Feed.SEC._sys.Rows.Utils;
using Feed.SEC._sys.RowsStringy;

namespace Feed.SEC._sys.Rows;





sealed record SubRowKey(string Adsh)
{
	public override string ToString() => Adsh;
	public object ToDump() => $"{this}";
}


/// <param name="Key.Adsh">0000002178-24-000076</param>
/// <param name="Cik">2969</param>
/// <param name="Name">ASSOCIATED BANC-CORP</param>
/// <param name="Sic">6022. Use SicCodeExt.FmtSic to get its description</param>
/// <param name="Former">ARCHER DANIELS MIDLAND CO</param>
/// <param name="Changed">19790917</param>
/// <param name="Afs">2-ACC</param>
/// <param name="Wksi">0/1</param>
/// <param name="Fye">0930</param>
/// <param name="Form">10-Q</param>
/// <param name="Period">20240630. We filter out the 6 ones that are null. It is always the last day of the month</param>
/// <param name="Fy">2024</param>
/// <param name="Fp">Q2</param>
/// <param name="Filed">20240814</param>
/// <param name="Accepted">2024-08-07 15:07:00</param>
/// <param name="Prevrpt">0/1</param>
/// <param name="Detail">0/1</param>
/// <param name="Instance">adp-20240630_htm.xml</param>
sealed record SubRow(
	SubRowKey Key,
	string Cik,
	string Name,
	string? Sic,
	string? Former,
	DateOnly? Changed,
	FilerStatus Afs,
	bool Wksi,
	string Fye,
	string Form,
	DateOnly Period,
	int Fy,
	string Fp,
	DateOnly Filed,
	DateTime Accepted,
	bool Prevrpt,
	bool Detail,
	string Instance
) : IRow<SubRow>
{
	public static string NameInArchive => "sub.txt";
	public static int ColumnCount => 36;
	public static string HeaderLine => "adsh	cik	name	sic	countryba	stprba	cityba	zipba	bas1	bas2	baph	countryma	stprma	cityma	zipma	mas1	mas2	countryinc	stprinc	ein	former	changed	afs	wksi	fye	form	period	fy	fp	filed	accepted	prevrpt	detail	instance	nciks	aciks";

	public static bool IsParsable(SubStringyRow e) =>
		e.Key.Adsh.Is_String() &&
		e.Cik.Is_String() &&
		e.Name.Is_String() &&
		e.Sic.Is_NullableString() &&
		e.Former.Is_NullableString() &&
		e.Changed.Is_NullableDateOnlyCompact() &&
		e.Afs.Is_FilerStatus() &&
		e.Wksi.Is_Bool() &&
		e.Fye.Is_String() &&
		e.Form.Is_String() &&
		e.Period.Is_DateOnlyCompact() &&
		e.Fy.Is_Int() &&
		e.Fp.Is_String() &&
		e.Filed.Is_DateOnlyCompact() &&
		e.Accepted.Is_DateTime() &&
		e.Prevrpt.Is_Bool() &&
		e.Detail.Is_Bool() &&
		e.Instance.Is_String() &&
		Quarter.CanParseSubRowQuarter(e.Fy, e.Fp) &&
		AssertChecks(e);


	// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
	static bool AssertChecks(SubStringyRow e)
	{
		if (e.Former == string.Empty ^ e.Changed == string.Empty) throw new ArgumentException("[Sub] Former and Changed fields should both have a value or both not have a value");

		return true;
	}


	public Quarter Quarter { get; } = Quarter.ParseSubRowQuarter(Fy, Fp);
	public string LinkFolder => $"https://www.sec.gov/Archives/edgar/data/{Cik}/{Key.Adsh.Replace("-", "")}/";

	// @formatter:off
	public static SubRow Parse(string[] xs) => new(
		Key:			new SubRowKey(xs[0].As_String()),
		Cik:			xs[1].As_String(),
		Name:			xs[2].As_String(),
		Sic:			xs[3].As_NullableString(),
		Former:			xs[20].As_NullableString(),
		Changed:		xs[21].As_NullableDateOnlyCompact(),
		Afs:			xs[22].As_FilerStatus(),
		Wksi:			xs[23].As_Bool(),
		Fye:			xs[24].As_String(),
		Form:			xs[25].As_String(),
		Period:			xs[26].As_DateOnlyCompact(),
		Fy:				xs[27].As_Int(),
		Fp:				xs[28].As_String(),
		Filed:			xs[29].As_DateOnlyCompact(),
		Accepted:		xs[30].As_DateTime(),
		Prevrpt:		xs[31].As_Bool(),
		Detail:			xs[32].As_Bool(),
		Instance:		xs[33].As_String()
	);
	// @formatter:on

	public string?[] Fields => [
		Key.Adsh.Fmt_String(),
		Cik.Fmt_String(),
		Name.Fmt_String(),
		Sic.Fmt_NullableString(),
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		Former.Fmt_NullableString(),
		Changed.Fmt_NullableDateOnlyCompact(),
		Afs.Fmt_FilerStatus(),
		Wksi.Fmt_Bool(),
		Fye.Fmt_String(),
		Form.Fmt_String(),
		Period.Fmt_DateOnlyCompact(),
		Fy.Fmt_Int(),
		Fp.Fmt_String(),
		Filed.Fmt_DateOnlyCompact(),
		Accepted.Fmt_DateTime(),
		Prevrpt.Fmt_Bool(),
		Detail.Fmt_Bool(),
		Instance.Fmt_String(),
		null,
		null,
	];


	public object ToDump() => new
	{
		Adsh = Key,
		Cik,
		Name,
		Sic,
		Former,
		Changed,
		Afs,
		Wksi,
		Fye,
		Form,
		Period,
		//Fy,
		//Fp,
		Quarter,
		Filed,
		Accepted,
		Prevrpt,
		Detail,
		Instance,
	};
}



