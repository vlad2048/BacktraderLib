namespace BacktraderLib._sys;

static class StringUtils
{
	public static string JoinLines(this IEnumerable<string> source) => string.Join(Environment.NewLine, source);
	public static string[] Chop(this string e, char sep = ',') => e.Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}