//global using Report = System.Collections.Generic.Dictionary<string, Feed.Trading212.FieldVal>;
//global using ReportSerie = System.Collections.Generic.SortedDictionary<BaseUtils.Quarter, System.Collections.Generic.Dictionary<string, Feed.Trading212.FieldVal>>;
//global using CompanyReports = System.Collections.Generic.Dictionary<Feed.Trading212.ReportType, System.Collections.Generic.SortedDictionary<BaseUtils.Quarter, System.Collections.Generic.Dictionary<string, Feed.Trading212.FieldVal>>>;


namespace Feed.Trading212;

public enum FieldType
{
	Currency = 1,
	Percent = 2,
	Number = 3,
	Invalid_Text = 4,
}

public sealed record FieldDef(
	string Title,
	FieldType Type,
	bool Important,
	bool Compact,
	FieldDef[] Kids
);

public sealed record FieldVal(
	decimal Value,
	string? Ccy
)
{
	public static readonly FieldVal Empty = new(0, null);
}



public sealed record FieldDefFlat(
	ReportType ReportType,
	int Idx,
	string Title,
	FieldType Type,
	bool Important,
	bool Compact
);


public static class FieldUtils
{
	public static IEnumerable<FieldDefFlat> Flatten(this Template template) =>
		from kv in template.Reports
		let reportType = kv.Key
		let refFields = kv.Value
		let flatFields = refFields.Flatten(reportType)
		from flatField in flatFields
		select flatField;

	static IEnumerable<FieldDefFlat> Flatten(this IEnumerable<FieldDef> xs, ReportType reportType)
	{
		var idx = 0;
		foreach (var x in xs)
		{
			yield return x.ToFieldDefFlat(reportType, idx++);
			foreach (var kid in x.Kids.Flatten(reportType))
				yield return kid;
		}
	}

	static FieldDefFlat ToFieldDefFlat(this FieldDef x, ReportType reportType, int idx) => new(reportType, idx, x.Title, x.Type, x.Important, x.Compact);
}