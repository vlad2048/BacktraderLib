namespace FeedUtils;

public static class StringExt
{
	public static string RemoveDoubleQuotes(this string s) =>
		(s.Length >= 2 && s.StartsWith('"') && s.EndsWith('"')) switch
		{
			true => s[1..^1],
			false => throw new ArgumentException("Missing quotes"),
		};
}