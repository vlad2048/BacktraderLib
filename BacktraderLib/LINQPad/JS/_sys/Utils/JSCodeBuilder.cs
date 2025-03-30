using BaseUtils;

namespace BacktraderLib._sys.Utils;

static class JSCodeBuilder
{
	static string Marker(int loc) => $"____{loc}____";

	// @formatter:off
	public static string JSRepl_Var(this string c, int i, string x) => c.Repl(i, x);
	public static string JSRepl_Arr(this string c, int i, int[] xs) => c.Repl(i, xs.FmtArr());
	public static string JSRepl_Arr(this string c, int i, double[] xs) => c.Repl(i, xs.FmtArr());
	public static string JSRepl_Arr(this string c, int i, DateTime[] xs) => c.Repl(i, xs.FmtArr(e => $"{e:yyyy-MM-dd}".Quote()));
	public static string JSRepl_Val(this string c, int i, string? x) => c.Repl(i, x != null ? x.Quote() : "null");
	public static string JSRepl_Val(this string c, int i, double x) => c.Repl(i, $"{x}");
	// @formatter:on
	public static string JSRepl_Obj(this string c, int i, string x)
	{
		// ReSharper disable once EmptyEmbeddedStatement
		while (JSRepl_Obj_Once(ref c, i, x)) ;
		return c;
	}

	public static string JSRepl_ArrOfObj<T>(this string c, int i, T[] xs) => c.JSRepl_Obj(i, xs.PlotlySer());



	static string Repl(this string s, int i, string t) => s.Replace(Marker(i), t);

	static bool JSRepl_Obj_Once(ref string src, int i, string dst)
	{
		var marker = Marker(i);
		var srcLines = src.SplitLines();
		var dstLines = dst.SplitLines();
		
		var srcLineIdx = srcLines.Index().FirstOrDefault(t => t.Item2.Contains(marker, StringComparison.Ordinal), (-1, string.Empty)).Item1;
		if (srcLineIdx == -1) return false;

		if (dstLines.Length <= 1)
		{
			src = src.ReplaceFirst(marker, dst);
			return true;
		}

		var srcLine = srcLines[srcLineIdx];
		var idx = srcLine.IndexOf(marker, StringComparison.Ordinal);
		if (idx == -1) throw new ArgumentException("Impossible");

		var leading = srcLine[..idx];
		if (!leading.All(e => e is '\t' or ' '))
		{
			src = src.ReplaceFirst(marker, dst);
			return true;
		}

		dstLines = dstLines
			.Index()
			.SelectA(t => t.Index switch
			{
				0 => t.Item,
				_ => leading + t.Item,
			});
		var dstIndented = dstLines.JoinLines();

		src = src.ReplaceFirst(marker, dstIndented);
		return true;
	}

	static string ReplaceFirst(this string str, string prev, string next)
	{
		var idx = str.IndexOf(prev, StringComparison.Ordinal);
		if (idx == -1) throw new ArgumentException("Impossible");
		var res = str[..idx] + next + str[(idx + prev.Length)..];
		return res;
	}


	static string FmtArr<T>(this T[] xs, Func<T, string>? fmt = null) => $"[{string.Join(", ", xs.Select(x => x.FmtItem(fmt)))}]";
	static string Quote(this string s) => $"'{s}'";
	static string FmtItem<T>(this T x, Func<T, string>? fmt) =>
		fmt switch
		{
			null => $"{x}",
			not null => fmt(x),
		};
}