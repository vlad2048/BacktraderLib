using BaseUtils;

namespace Feed.Trading212;


public sealed record FieldId(ReportType ReportType, string Title);

public sealed record FieldVal(Quarter Quarter, double Value);



public static class FieldUtils
{
	public static string[] GetFieldTitles(this ScrapeData data, ReportType reportType)
	{
		IEnumerable<string> Rec(RefField field)
		{
			yield return field.Title;
			if (field.Rows != null)
				foreach (var subField in field.Rows)
				{
					foreach (var recField in Rec(subField))
						yield return recField;
				}
		}

		return data.Reports[reportType].Last().Value.SelectManyA(Rec);
	}


	public static FieldVal[] GetFieldValues(this ScrapeData data, FieldId fieldId)
	{
		RefField? Rec(RefField field) =>
			(field.Title == fieldId.Title) switch
			{
				true => field,
				false => field.Rows switch
				{
					not null => field.Rows.Select(Rec).FirstOrDefault(e => e != null),
					null => null,
				},
			};

		return data.Reports[fieldId.ReportType]
			.SelectA(kv => new FieldVal(
				kv.Key,
				ToDouble(kv.Value.Select(Rec).First(e => e != null)?.Value.Value)
			));
	}

	static double ToDouble(decimal? v) =>
		v.HasValue switch
		{
			true => (double)v.Value,
			false => double.NaN,
		};
}