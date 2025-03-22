namespace BacktraderLib._sys.Utils;

static class JSIndentUtils
{
	public static string JSIndent(this string str, int n)
	{
		var pad = new string('\t', n);
		return str
			.SplitInLines()
			.Select((e, i) => i switch
			{
				0 => e,
				_ => pad + e,
			})
			.JoinLines();
	}

	public static string JSIndentAll(this string str, int n)
	{
		var pad = new string('\t', n);
		return str
			.SplitInLines()
			.Select(e => pad + e)
			.JoinLines();
	}

	static string[] SplitInLines(this string? str) => str == null ? Array.Empty<string>() : str.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
}