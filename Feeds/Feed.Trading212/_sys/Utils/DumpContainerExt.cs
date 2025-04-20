using LINQPad;

namespace Feed.Trading212._sys.Utils;

static class DumpContainerExt
{
	public static void Log(this DumpContainer dc, string s) => dc.AppendContent(s);
	public static void LogH2(this DumpContainer dc, string s) => dc.AppendContent(Util.RawHtml($"<h2>{s}</h2>"));
	public static void LogH3(this DumpContainer dc, string s) => dc.AppendContent(Util.RawHtml($"<h3>{s}</h3>"));
	public static void LogH4(this DumpContainer dc, string s) => dc.AppendContent(Util.RawHtml($"<h4>{s}</h4>"));

	public static DumpContainer AddNewDC(this DumpContainer dc, string? header)
	{
		if (header != null)
			dc.LogH3(header);
		var dcNew = new DumpContainer();
		dc.AppendContent(dcNew);
		return dcNew;
	}
}