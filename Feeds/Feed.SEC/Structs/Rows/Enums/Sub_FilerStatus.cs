namespace Feed.SEC;

public enum FilerStatus
{
	NotAssigned,
	LargeAccelerated,
	Accelerated,
	SmallerReportingAccelerated,
	NonAccelerated,
	SmallerReportingFiler,
}

static class FilerStatusUtils
{
	public static bool Is_FilerStatus(this string s) => s is "" or "1-LAF" or "2-ACC" or "3-SRA" or "4-NON" or "5-SML";
	public static FilerStatus As_FilerStatus(this string s) => s switch
	{
		"" => FilerStatus.NotAssigned,
		"1-LAF" => FilerStatus.LargeAccelerated,
		"2-ACC" => FilerStatus.Accelerated,
		"3-SRA" => FilerStatus.SmallerReportingAccelerated,
		"4-NON" => FilerStatus.NonAccelerated,
		"5-SML" => FilerStatus.SmallerReportingFiler,
		_ => throw new ArgumentException($"Not a recognized FilerStatus: '{s}'"),
	};
	public static string Fmt_FilerStatus(this FilerStatus s) => s switch
	{
		FilerStatus.NotAssigned => "",
		FilerStatus.LargeAccelerated => "1-LAF",
		FilerStatus.Accelerated => "2-ACC",
		FilerStatus.SmallerReportingAccelerated => "3-SRA",
		FilerStatus.NonAccelerated => "4-NON",
		FilerStatus.SmallerReportingFiler => "5-SML",
		_ => throw new ArgumentException($"Not a recognized FilerStatus: '{s}'"),
	};
}