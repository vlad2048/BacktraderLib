using BaseUtils;

namespace Feed.Trading212;


/*

RefValue.Value == null		<=>		RefValue == RefValue.Empty
RefValue.Type == Text		<=>		RefValue == RefValue.Empty
RefValue.Type == Currency	<=>		RefValue.Currency != null
RefValue.Type != RefValueType.Currency || RefValue.Currency!.Length == 3

RefValue.Currency is one of
	USD
	CAD
	HKD
	CNY
	MYR
	EUR
	JPY
	BRL
	ILS
	AUD
	GBP
	SGD
	CHF
	TWD

Usually RefValue.Currency is unique for a company (except for 49 companies that have 2 currencies)

Code to verify the format is consistent across companies is commented at the bottom

*/


public sealed record RefField(
	string Title,
	RefValue Value,
	bool Important,
	RefField[]? Rows
);

public enum RefValueType
{
	Currency = 1,
	Percent = 2,
	Number = 3,
	Text = 4,
}

public sealed record RefValue(
	decimal? Value,
	bool Compact,
	string? Currency,
	RefValueType Type
)
{
	public static readonly RefValue Empty = new(null, false, null, RefValueType.Text);
	public override string ToString() => this.Fmt();
	public object ToDump() => ToString();
}



static class RefFieldUtils
{
	public static RefField[] Flatten(this RefField[] xs)
	{
		var list = new List<RefField>();
		void Rec(RefField x)
		{
			list.Add(x);
			if (x.Rows != null) foreach (var row in x.Rows) Rec(row);
		}
		foreach (var x in xs) Rec(x);
		return [.. list];
	}

	public static FieldDef[] ToFields(this RefField[] fields) => fields.SelectA(ToField);

	public static FieldDef ToField(this RefField e) => new(
		e.Title,
		(FieldType)(int)e.Value.Type,
		e.Important,
		e.Value.Compact,
		e.Rows switch
		{
			not null => e.Rows.SelectA(ToField),
			null => [],
		}
	);

	public static bool IsEmpty(this FieldDef e) => e.Type == FieldType.Invalid_Text;
}



file static class RefValueFormatter
{
	public static string Fmt(this RefValue e) =>
		e.Type switch
		{
			RefValueType.Currency => FmtCurrency(e.Value.Force(), e.Currency.Force()),
			RefValueType.Percent => $"{e.Value.Force() * 100:F2}%",
			RefValueType.Number => e.Value.Force().FmtHuman(),
			RefValueType.Text => "_",
			_ => throw new ArgumentException($"Unknown RefValueType: {e.Type}"),
		};

	static decimal Force(this decimal? v) => v ?? throw new ArgumentException("Missing Value");
	static string Force(this string? v) => v ?? throw new ArgumentException("Missing Currency");

	static string FmtCurrency(decimal v, string ccy) =>
		ccy switch
		{
			"USD" => $"${v.FmtHuman()}",
			_ => $"{v.FmtHuman()}{ccy}",
		};
}









/*


// ******************************
// * Code to verify consistency *
// ******************************


#load "..\backtrader-lib"
using static BacktraderLib.CtrlsUtilsStatic;


void Main()
{
	var fmt = FormatExtractor.ExtractFormat();

	//RefFieldUtils.VerifyAssumptions(); return;

	API.GetCompanies()
		.ShowProgress()
		.Select(e => (companyName:e, data:API.Load(e)))
		.ForEach(t => FormatVerifier.VerifyConforms(t.data, fmt, t.companyName, false));
}







public sealed record Field(
	string Title,
	bool Important,
	bool Compact,
	RefValueType Type,
	string? Currency,
	Field[] Kids
);

public sealed record Format(Dictionary<ReportType, Field[]> Map);








public static class FieldUtils
{
	public static IEnumerable<Field> Flatten(this IEnumerable<Field> xs)
	{
		foreach (var x in xs)
		{
			yield return x;
			foreach (var kid in x.Kids.Flatten())
				yield return kid;
		}
	}
}





public static class FormatExtractor
{
	public static Format ExtractFormat()
	{
		var companies = API.GetCompanies();
		var data = API.Load(companies[0]);

		var map =
			Enum.GetValues<ReportType>().ToDictionary(
				e => e,
				e => GetFields(data.Reports[e].First().Value)
					.SelectA(f => f.FindActualType(e, companies))
			);

		return new Format(map);
	}




	static Field FindActualType(this Field field, ReportType reportType, string[] companies)
	{
		if (!field.IsEmpty()) return field with { Kids = field.Kids.SelectA(e => FindActualType(e, reportType, companies)) };

		RefField Find() => (
			from company in companies
			let data = API.Load(company)
			from t in data.Reports
			where t.Key == reportType
			let xss = t.Value.Values
			from xs in xss
			let fieldsOther = xs.Flatten()
			from fieldOther in fieldsOther
			where field.Title == fieldOther.Title && field.Important == fieldOther.Important && ((field.Kids.Length == 0 && fieldOther.Rows == null) || (field.Kids.Length == fieldOther.Rows!.Length))
			where fieldOther.Value != RefValue.Empty
			select fieldOther
		).First();

		var good = Find().ToField();
		var res = field with
		{
			Compact = good.Compact,
			Currency = good.Currency,
			Type = good.Type,
			Kids = field.Kids.SelectA(e => FindActualType(e, reportType, companies)),
		};
		return res;
	}


	static Field[] GetFields(RefField[] fields) => fields.SelectA(ToField);
	static Field ToField(this RefField e) => new(
			e.Title,
			e.Important,
			e.Value.Compact,
			e.Value.Type,
			e.Value.Currency,
			e.Rows switch
			{
				not null => e.Rows.SelectA(f => ToField(f)),
				null => [],
			}
		);
	
	static bool IsEmpty(this Field e) => e.Type == RefValueType.Text;
}



public static class FormatVerifier
{
	public static void VerifyConforms(ScrapeData data, Format fmt, string companyName, bool preCheck)
	{
		void Verify(RefField[] xsRef, Field[] ys, ReportType reportType)
		{
			static string fmt(Field e) => $"title:{e.Title}  important:{e.Important}  compact:{e.Compact}  type:{e.Type}  ccy:{e.Currency}  kids:{e.Kids.Length}";

			var xs = GetFields(xsRef);
			void Verify(Field x, Field y)
			{
				var isMatch = x.IsEmpty() switch
				{
					true => true,
					false => preCheck switch
					{
						true => CmpPreCheck(x, y),
						false => CmpFull(x, y),
					},
				};
				if (!isMatch) throw new ArgumentException($"""
				Format mismatch
				===============
				Company  : '{companyName}'
				Report   : {reportType}
				Field Act: {fmt(x)}
				Field Exp: {fmt(y)}
				""");
				for (var i = 0; i < x.Kids.Length; i++)
					Verify(x.Kids[i], y.Kids[i]);
			}
			if (xsRef.Length != ys.Length) throw new ArgumentException($"""
			Format mismatch
			===============
			Company  : '{companyName}'
			Report   : {reportType}
			RootFieldCount Act: {xs.Length}
			RootFieldCount Exp: {ys.Length}
			""");
			for (var i = 0; i < xs.Length; i++)
				Verify(xs[i], ys[i]);
		}
		foreach (var (reportType, quarters) in data.Reports)
		{
			var ys = fmt.Map[reportType];
			foreach (var (quarter, xsRef) in quarters)
				Verify(xsRef, ys, reportType);
		}
	}
	
	static bool CmpPreCheck(Field x, Field y) =>
		x.Title == y.Title &&
		x.Important == y.Important &&
		x.Kids.Length == y.Kids.Length;


	static bool CmpFull(Field x, Field y) =>
		x.Title == y.Title &&
		x.Important == y.Important &&
		x.Kids.Length == y.Kids.Length &&
		x.Compact == y.Compact &&
		!(x.Currency == null ^ y.Currency == null) &&
		x.Type == y.Type;

	static Field[] GetFields(RefField[] fields) => fields.SelectA(ToField);
	static Field ToField(this RefField e) => new(
			e.Title,
			e.Important,
			e.Value.Compact,
			e.Value.Type,
			e.Value.Currency,
			e.Rows switch
			{
				not null => e.Rows.SelectA(f => ToField(f)),
				null => [],
			}
		);

	static bool IsEmpty(this Field e) => e.Type == RefValueType.Text;
}









public static class RefFieldUtils
{
	public static void VerifyAssumptions()
	{
		API.GetCompanies()
			.ShowProgress()
			.ForEach(company =>
			{
				var data = API.Load(company);
				var fields = data.GetAllRefFieldsFlatten().SelectA(e => e.field);
				
				fields.Verify(
					e => e.Value.Value != null ^ e.Value == RefValue.Empty,
					"RefField.Value.Value == null  <=>  RefField.Value == RefValue.Empty"
				);
				
				fields.Verify(
					e => !(e.Value.Type == RefValueType.Text ^ e.Value == RefValue.Empty),
					"RefField.Value.Type == Text  <=>  RefField.Value == RefValue.Empty"
				);
			});
	}
	
	static void Verify(this RefField[] fields, Func<RefField, bool> fun, string name)
	{
		if (fields.Any(e => !fun(e))) throw new ArgumentException($"Assumption broken: {name}");
	}

	static (ReportType reportType, RefField field)[] GetAllRefFieldsFlatten(this ScrapeData data)
	{
		var list = new List<(ReportType, RefField)>();
		foreach (var (reportType, quarters) in data.Reports)
			foreach (var (quarter, xsRef) in quarters)
			{
				var fields = xsRef.Flatten();
				list.AddRange(fields.Select(e => (reportType, e)));
			}
		return [.. list];
	}

	public static RefField[] Flatten(this RefField[] xs)
	{
		var list = new List<RefField>();
		void Rec(RefField x)
		{
			list.Add(x);
			if (x.Rows != null) foreach (var row in x.Rows) Rec(row);
		}
		foreach (var x in xs) Rec(x);
		return [.. list];
	}
}








*/