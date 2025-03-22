namespace BacktraderReverser.Utils;

static class ParseUtils
{
	public static int ParseDate(this string s) => (DateTime.Parse(s) - Consts.StartDate).Days;

	public static double? ParseDoubleOpt(this string s) => s switch
	{
		"_" => null,
		_ => double.Parse(s),
	};

	public static double ParseDouble(this string s) => double.Parse(s);

	public static int ParseInt(this string s) => int.Parse(s);

	public static bool ParseBool(this string s) => bool.Parse(s);

	public static E ParseEnum<E>(this string s) where E : struct, Enum => Enum.Parse<E>(s);
}