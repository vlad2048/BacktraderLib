using BaseUtils;
using ArgumentException = System.ArgumentException;

namespace Feed.Trading212._sys._2_Processing.Logic;

static class TemplateVerifier
{
	public static void Verify(ScrapeData data, Template template, string companyName)
	{
		var typesData = data.Reports.Keys.OrderBy(e => e).ToArray();
		var typesTemp = template.Reports.Keys.OrderBy(e => e).ToArray();
		if (!typesData.IsSame(typesTemp))
			// @formatter:off
			throw new ArgumentException($"""
			Template mismatch: wrong number of ReportTypes
			=================
			Company  : '{companyName}'
			Types    : {typesData.Length}
			Template : {typesTemp.Length}
			""");
			// @formatter:on

		void VerifyArr(RefField[] xsRef, FieldDef[] ys, ReportType reportType)
		{
			static string fmt(FieldDef e) => $"title:{e.Title}  important:{e.Important}  compact:{e.Compact}  type:{e.Type}  kids:{e.Kids.Length}";

			var xs = xsRef.ToFields();
			void VerifyOne(FieldDef x, FieldDef y)
			{
				var isMatch = x.IsEmpty() switch
				{
					true => true,
					false => Cmp(x, y),
				};

				// @formatter:off
				if (!isMatch) throw new ArgumentException($"""
				Template mismatch
				=================
				Company  : '{companyName}'
				Report   : {reportType}
				FieldDef Act: {fmt(x)}
				FieldDef Exp: {fmt(y)}
				""");
				// @formatter:on

				for (var i = 0; i < x.Kids.Length; i++)
					VerifyOne(x.Kids[i], y.Kids[i]);
			}

			// @formatter:off
			if (xsRef.Length != ys.Length) throw new ArgumentException($"""
			Template mismatch
			=================
			Company  : '{companyName}'
			Report   : {reportType}
			RootFieldCount Act: {xs.Length}
			RootFieldCount Exp: {ys.Length}
			""");
			// @formatter:on

			for (var i = 0; i < xs.Length; i++)
				VerifyOne(xs[i], ys[i]);
		}
		foreach (var (reportType, quarters) in data.Reports)
		{
			var ys = template.Reports[reportType];
			foreach (var (_, xsRef) in quarters)
				VerifyArr(xsRef, ys, reportType);
		}
	}



	public static bool Cmp(FieldDef x, FieldDef y) =>
		x.Title == y.Title &&
		x.Important == y.Important &&
		x.Kids.Length == y.Kids.Length &&
		x.Compact == y.Compact &&
		x.Type == y.Type;
}