namespace BaseUtils;

public static class StringExt
{
	public static string[] SplitLines(this string str) => str.Split(Environment.NewLine);
	public static string JoinLines(this IEnumerable<string> source) => string.Join(Environment.NewLine, source);

	public static string JoinText<T>(this IEnumerable<T> source, string separator) => string.Join(separator, source);
}