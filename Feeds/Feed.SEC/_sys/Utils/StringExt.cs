namespace Feed.SEC._sys.Utils;

static class StringExt
{
	public static string Ellipse(this string? str, int n) =>
		str switch
		{
			not null => (str.Length > n) switch
			{
				true => str[..(n - 4)] + " ...",
				false => str,
			},
			null => "_",
		};
}