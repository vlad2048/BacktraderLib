using BaseUtils;

namespace Feed.Trading212._sys._2_Processing.Logic;

static class ReportCompiler
{
	public static CompanyReports Compile(ScrapeData data, Template template, string companyName)
	{
		TemplateVerifier.Verify(data, template, companyName);

		return new CompanyReports(
			data.Reports
				.ToDictionary(
					e => e.Key,
					e => e.Value.ToSortedDictionary(
						f => f,
						f => f.Zip(template.Reports[e.Key])
							.SelectA(t => Check(t, companyName, e.Key))
					)
				)
		);
	}

	static FieldVal Check((RefField fieldDataRef, FieldDef fieldTemplate) t, string companyName, ReportType reportType)
	{
		if (t.fieldDataRef.Value == RefValue.Empty)
			return FieldVal.Empty;

		var fieldData = t.fieldDataRef.ToField();
		if (!TemplateVerifier.Cmp(fieldData, t.fieldTemplate))
			throw new ArgumentException($"""
			Unexpected Template mismatch in ReportCompiler
			==============================================
			Company  : '{companyName}'
			Report   : {reportType}
			FieldDef Act: {fieldData}
			FieldDef Exp: {t.fieldTemplate}
			""");
		var value = t.fieldDataRef.Value.Value ?? throw new ArgumentException($"""
			Unexpected missing Value in ReportCompiler
			==========================================
			Company  : '{companyName}'
			Report   : {reportType}
			FieldDef Act: {fieldData}
			FieldDef Exp: {t.fieldTemplate}
			""");
		var ccy = t.fieldDataRef.Value.Currency;
		return new FieldVal(value, ccy);
	}
}