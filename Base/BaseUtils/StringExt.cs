namespace BaseUtils;

public static class StringExt
{
	public static string[] SplitLines(this string str) => str.Split(Environment.NewLine);
	public static string JoinLines(this IEnumerable<string> source) => string.Join(Environment.NewLine, source);

	public static string JoinText<T>(this IEnumerable<T> source, string separator) => string.Join(separator, source);

	public static string Truncate(this string str, int lng) => str switch
	{
		null => throw new ArgumentException(),
		_ when str.Length <= lng => str,
		_ => str[..lng]
	};

	public static string RemovePrefix(this string s, string prefix) =>
		s.StartsWith(prefix) switch
		{
			true => s[prefix.Length..],
			false => throw new ArgumentException($"Missing prefix '{prefix}' in '{s}'"),
		};
}