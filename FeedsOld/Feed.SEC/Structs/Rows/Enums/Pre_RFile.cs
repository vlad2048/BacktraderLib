namespace Feed.SEC;

public enum RFile
{
	Htm,
	Xml,
}

static class RFileUtils
{
	public static bool Is_RFile(this string s) => s is "H" or "X";
	public static RFile As_RFile(this string s) => s switch
	{
		"H" => RFile.Htm,
		"X" => RFile.Xml,
		_ => throw new ArgumentException($"Unrecognized [Pre].RFile: '{s}'"),
	};
	public static string Fmt_RFile(this RFile s) => s switch
	{
		RFile.Htm => "H",
		RFile.Xml => "X",
		_ => throw new ArgumentException($"Unrecognized [Pre].RFile: '{s}'"),
	};
}