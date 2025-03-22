namespace BaseUtils;

public static class StringExt
{
	public static string JoinText<T>(this IEnumerable<T> source, string separator) => string.Join(separator, source);
}