namespace LINQPadPlus._sys.Utils;

static class StringExt
{
	public static string[] SplitLines(this string str) => str.Split(Environment.NewLine);
	public static string JoinLines(this IEnumerable<string> source) => string.Join(Environment.NewLine, source);
	public static string JoinText(this IEnumerable<string> source, string sep) => string.Join(sep, source);
}