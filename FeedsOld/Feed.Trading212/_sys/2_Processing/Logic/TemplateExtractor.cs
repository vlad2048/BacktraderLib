using BaseUtils;

namespace Feed.Trading212._sys._2_Processing.Logic;

static class TemplateExtractor
{
	public static Template Extract()
	{
		var companies = API.Scraping.GetCompanies();
		if (companies.Length == 0)
			throw new ArgumentException("No companies found");

		var data = API.Scraping.Load(companies[0]);

		var map =
			Enum.GetValues<ReportType>().ToDictionary(
				e => e,
				e => data.Reports[e].First().Value
					.ToFields()
					.SelectA(f => f.FindActualType(e, companies))
			);

		return new Template(map);
	}


	static FieldDef FindActualType(this FieldDef field, ReportType reportType, string[] companies)
	{
		if (!field.IsEmpty()) return field with { Kids = field.Kids.SelectA(e => FindActualType(e, reportType, companies)) };

		RefField Find() => (
			from company in companies
			let data = API.Scraping.Load(company)
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
			Type = good.Type,
			Kids = field.Kids.SelectA(e => FindActualType(e, reportType, companies)),
		};
		return res;
	}
}