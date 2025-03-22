namespace BacktraderLib._sys;

static class FmtExt
{
	public static string FmtDumpVal(this double e) => $"{e:#.######}";					// at most 6 decimals
	//public static string FmtDumpVal(this double e) => $"{e:F6}";					// 6 decimals
	//public static string FmtDumpVal(this double e) => e == 0 ? "0" : $"{e:F6}";		// 6 decimals unless it's zero

	//public static string FmtDumpVal(this double e) => Smart(e);


	static string Smart(double e)
	{
		if (e == 0) return "0       ";
		var s = $"{e:#.######}";
		var idx = s.IndexOf('.');
		return idx switch
		{
			-1 => $"{s}       ",
			_ => $"{s}" + new string(' ', 7 - (s.Length - idx)),
		};
	}
}